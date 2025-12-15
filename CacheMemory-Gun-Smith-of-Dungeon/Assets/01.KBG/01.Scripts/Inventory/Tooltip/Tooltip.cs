using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using KBG.Item;
using UnityEngine.UI;

namespace KBG.Inventory
{
    [Serializable]
    public class TooltipTxt
    {
        public string key;
        public TextMeshProUGUI text;
        public TextMeshProUGUI amountText;

        public TooltipTxt(string key, TextMeshProUGUI text, TextMeshProUGUI amountText)
        {
            this.key = key;
            this.text = text;
            this.amountText = amountText;
        }
    }

    public class Tooltip : MonoSingleton<Tooltip>
    {
        public string gunName;

        [Header("Canvas RectTransform (Root Canvas)")]
        [SerializeField] private RectTransform canvas;

        [Header("Root Tooltip RectTransform (Optional)")]
        [SerializeField] private RectTransform tooltipRect; // 인스펙터에서 명시 연결 가능(권장)

        [SerializeField] private TextMeshProUGUI nameText;

        [Header("Status Text")]
        [SerializeField] private GameObject statusTextParent;
        [SerializeField] private List<TooltipTxt> statusTexts;

        [Header("Info")]
        [SerializeField] private GameObject infoTextParent;
        [SerializeField] private List<TooltipTxt> infoTexts;

        public RectTransform RectTransform { get; private set; }

        private Image image;

        protected override void Awake()
        {
            base.Awake();

            // RectTransform 캐싱 (1회)
            CacheRectTransform();
            OnValidate();
        }

        private void CacheRectTransform()
        {
            // 1) 인스펙터 지정 우선
            if (tooltipRect != null)
            {
                RectTransform = tooltipRect;
                return;
            }

            // 2) 같은 오브젝트에서 찾기
            RectTransform = GetComponent<RectTransform>();
            if (RectTransform != null) return;

            // 3) 자식에서 찾기 (루트가 빈 오브젝트인 케이스 대비)
            RectTransform = GetComponentInChildren<RectTransform>(true);

            if (RectTransform == null)
            {
                Debug.LogError(
                    "Tooltip: RectTransform을 찾지 못했습니다.\n" +
                    "- Tooltip 오브젝트가 Canvas 하위 UI인지 확인하거나\n" +
                    "- 실제 UI 패널(RectTransform)이 자식이라면 tooltipRect에 그 패널을 연결하세요."
                );
                enabled = false;
            }
        }

        private void SetPivot(Vector2 position)
        {
            Debug.Log($"[Tooltip] SetPivot caller obj='{gameObject.name}', id={gameObject.GetInstanceID()}, hasRect={(GetComponent<RectTransform>() != null)} parent='{transform.parent?.name}'");

            if (!enabled) return;
            if (RectTransform == null) return;

            // canvas는 “Canvas 오브젝트의 RectTransform”이 들어가야 함
            if (canvas == null)
            {
                Debug.LogError("Tooltip: canvas(RectTransform)가 비어있습니다. Canvas의 RectTransform을 연결하세요.");
                return;
            }

            // 툴팁 크기(스케일 포함)
            Vector2 size = RectTransform.sizeDelta;
            Vector3 scale = RectTransform.lossyScale; // localScale보다 안전(상위 스케일 영향 반영)

            float w = size.x * scale.x;
            float h = size.y * scale.y;

            // 화면 기준: position(마우스 위치) 기준으로 툴팁이 캔버스 밖으로 나가면 pivot을 뒤집는다
            // RectTransformUtility는 "ScreenPoint" 기준이므로 position은 screenPosition이어야 함.
            Vector2 checkRightTop = position + new Vector2(w, h);

            bool outRight = !RectTransformUtility.RectangleContainsScreenPoint(canvas, checkRightTop);
            bool outTop = !RectTransformUtility.RectangleContainsScreenPoint(canvas, checkRightTop);

            RectTransform.pivot = new Vector2(outRight ? 1f : 0f, outTop ? 1f : 0f);
            RectTransform.anchoredPosition = position;
        }

        public void OpenTooltip(Part part, Vector2 position)
        {
            SetPivot(position);

            if (part == null || gameObject.activeSelf) return;

            gameObject.SetActive(true);

            var ingredient = part.partData.ingredients.First(i => i.requiredIngredient == part.madeBy);

            nameText.text = part.partData.itemName;

            foreach (var effect in ingredient.effects)
            {
                foreach (var text in statusTexts)
                {
                    if (text.key == effect.effectType.ToString())
                    {
                        text.amountText.gameObject.SetActive(true);
                        text.text.gameObject.SetActive(true);
                        text.amountText.text = (effect.effectAmount > 0 ? "+" : "") + effect.effectAmount;
                    }
                    else
                    {
                        text.amountText.gameObject.SetActive(false);
                        text.text.gameObject.SetActive(false);
                    }
                }
            }

            {
                PartData.PartEffect effect =
                    ingredient.effects.FirstOrDefault(e => e.effectType == PartEffectType.Capacity);
                var capacity = infoTexts.First(e => e.key == nameof(PartEffectType.Capacity));

                if (effect == null)
                {
                    capacity.amountText.gameObject.SetActive(false);
                    capacity.text.gameObject.SetActive(false);
                }
                else
                {
                    capacity.amountText.gameObject.SetActive(true);
                    capacity.text.gameObject.SetActive(true);
                    capacity.amountText.text = effect.effectAmount.ToString();
                }

                string text = "";
                switch (part.madeBy)
                {
                    case IngredientType.Polyamide:
                        text = "폴리아마이드";
                        break;
                    case IngredientType.Steel:
                        text = "강철";
                        break;
                    case IngredientType.StainlessSteel:
                        text = "스테인리스강";
                        break;
                }

                infoTexts.First(t => t.key == "Ingredient").amountText.text = text;
                infoTexts.First(t => t.key == "Durability").amountText.text = part.durability + "/" + ingredient.durability;
            }
        }

        public void OpenTooltip(GunData data, Vector2 position)
        {
            SetPivot(position);

            if (data == null || gameObject.activeSelf) return;

            gameObject.SetActive(true);

            foreach (var status in statusTexts)
            {
                status.text.gameObject.SetActive(true);
                status.amountText.gameObject.SetActive(true);
                if (PartEffectType.TryParse(status.key, out PartEffectType effectType))
                    status.amountText.text = data.GetStatus(effectType).ToString();
            }

            infoTexts.First(t => t.key == nameof(PartEffectType.Capacity)).amountText.gameObject.SetActive(true);
            infoTexts.First(t => t.key == nameof(PartEffectType.Capacity)).text.gameObject.SetActive(true);
            infoTexts.First(t => t.key == nameof(PartEffectType.Capacity)).amountText.text =
                data.GetStatus(PartEffectType.Capacity).ToString();

            infoTexts.First(t => t.key == "Ingredient").amountText.gameObject.SetActive(false);
            infoTexts.First(t => t.key == "Ingredient").text.gameObject.SetActive(false);
            infoTexts.First(t => t.key == "Durability").amountText.gameObject.SetActive(false);
            infoTexts.First(t => t.key == "Durability").text.gameObject.SetActive(false);

            nameText.text = (GunDataApplier.Instance.gunStatusData.CheckEndModding() ? gunName : "미완성 " + gunName);
        }

        private void OnValidate()
        {
            if (statusTextParent)
            {
                statusTexts = new List<TooltipTxt>();
                int i = 0;
                foreach (var stat in Enum.GetValues(typeof(PartEffectType)))
                {
                    string statName = stat.ToString();
                    if (statName == nameof(PartEffectType.Capacity)) continue;

                    statusTexts.Add(new TooltipTxt(
                        statName,
                        statusTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>(),
                        statusTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>()
                    ));
                }
            }

            if (infoTextParent)
            {
                infoTexts = new List<TooltipTxt>();
                int i = 0;
                infoTexts.Add(new TooltipTxt(
                    nameof(PartEffectType.Capacity),
                    infoTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>(),
                    infoTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>()
                ));
                infoTexts.Add(new TooltipTxt(
                    "Ingredient",
                    infoTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>(),
                    infoTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>()
                ));
                infoTexts.Add(new TooltipTxt(
                    "Durability",
                    infoTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>(),
                    infoTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>()
                ));
            }
        }
    }
}
