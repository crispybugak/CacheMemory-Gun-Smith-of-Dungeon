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
        [SerializeField] private RectTransform canvas;
        [SerializeField] private TextMeshProUGUI nameText;
        
        [Header("Status Text")]
        [SerializeField] private GameObject statusTextParent;
        [SerializeField] private List<TooltipTxt> statusTexts;
        
        [Header("Info")]
        [SerializeField] private GameObject infoTextParent;
        [SerializeField] private List<TooltipTxt> infoTexts;

        public RectTransform RectTransform{get; private set;}
        private Image image;

        protected override void Awake()
        {
            base.Awake();
            OnValidate();
        }

        private void SetPivot( Vector2 position )
        {
            RectTransform = GetComponent<RectTransform>();
            RectTransform.pivot = new Vector2(
                RectTransformUtility.RectangleContainsScreenPoint(canvas,
                    new Vector2(RectTransform.sizeDelta.x*RectTransform.localScale.x + RectTransform.anchoredPosition.x,
                        RectTransform.sizeDelta.y + RectTransform.anchoredPosition.y )) ? 0 : 1,
                RectTransformUtility.RectangleContainsScreenPoint(canvas,
                    new Vector2(RectTransform.sizeDelta.x + RectTransform.anchoredPosition.x,
                        RectTransform.sizeDelta.y*RectTransform.localScale.y + RectTransform.anchoredPosition.y)) ? 0 : 1);
            RectTransform.anchoredPosition = position;
        }
        
        public void OpenTooltip(Part part, Vector2 position)
        {
            SetPivot(position);
            
            if (part == null || gameObject.activeSelf ) return;
            
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
                        text.amountText.text = (effect.effectAmount > 0? "+":"")+effect.effectAmount;
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
                infoTexts.First(t => t.key == "Durability").amountText.text = part.durability+"/"+ingredient.durability;
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
            
            infoTexts.First(t => t.key ==  nameof(PartEffectType.Capacity)).amountText.gameObject.SetActive(true);
            infoTexts.First(t => t.key ==  nameof(PartEffectType.Capacity)).text.gameObject.SetActive(true);
            infoTexts.First(t => t.key ==  nameof(PartEffectType.Capacity)).amountText.text = data.GetStatus(PartEffectType.Capacity).ToString();
            infoTexts.First(t => t.key == "Ingredient").amountText.gameObject.SetActive(false);
            infoTexts.First(t => t.key == "Ingredient").text.gameObject.SetActive(false);
            infoTexts.First(t => t.key == "Durability").amountText.gameObject.SetActive(false);
            infoTexts.First(t => t.key == "Durability").text.gameObject.SetActive(false);

            nameText.text = (GunDataApplier.Instance.gunStatusData.CheckEndModding() ? gunName : "미완성 "+gunName);

        }

        private void OnValidate()
        {
            if (statusTextParent)
            {
                statusTexts = new  List<TooltipTxt>();
                int i = 0;
                foreach (var stat in Enum.GetValues(typeof(PartEffectType)))
                {
                    string statName = stat.ToString();
                    if (statName == nameof(PartEffectType.Capacity)) continue;
                    statusTexts.Add(new TooltipTxt(statName, statusTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>(), statusTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>()));
                }   
            }

            if (infoTextParent)
            {
                infoTexts = new  List<TooltipTxt>();
                int i = 0;
                infoTexts.Add(new TooltipTxt(nameof(PartEffectType.Capacity), infoTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>(), infoTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>()));
                infoTexts.Add(new TooltipTxt("Ingredient", infoTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>(), infoTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>()));
                infoTexts.Add(new TooltipTxt("Durability", infoTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>(), infoTextParent.transform.GetChild(i++).GetComponent<TextMeshProUGUI>()));
            }
        }
    }
}
