using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using KBG.Item;

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
        [SerializeField] private TextMeshProUGUI nameText;
        
        [Header("Status Text")]
        [SerializeField] private GameObject statusTextParent;
        [SerializeField] private List<TooltipTxt> statusTexts;
        
        [Header("Info")]
        [SerializeField] private GameObject infoTextParent;
        [SerializeField] private List<TooltipTxt> infoTexts;

        protected override void Awake()
        {
            base.Awake();
            OnValidate();
        }

        public void OpenTooltip(Part part)
        {
            var ingredient = part.partData.ingredients.First(i => i.requiredIngredient == part.madeBy);
            
            nameText.text = part.partData.itemName;
            foreach (var effect in ingredient.effects)
            {
                
                foreach (var text in statusTexts)
                {
                    if (text.key == effect.effectType.ToString())
                    {
                        text.amountText.enabled = true;
                        text.text.enabled = true;
                        text.amountText.text = (effect.effectAmount > 0? "+":"")+effect.effectAmount;
                    }
                    else
                    {
                        text.amountText.enabled = false;
                        text.text.enabled = false;
                    }
                }

            }

            {
                PartData.PartEffect effect =
                    ingredient.effects.FirstOrDefault(e => e.effectType == PartEffectType.Capacity);
                var capacity = infoTexts.First(e => e.key == nameof(PartEffectType.Capacity));
                if (effect == null)
                {
                    capacity.amountText.enabled = false;
                    capacity.text.enabled = false;
                }
                else
                {
                    capacity.amountText.enabled = true;
                    capacity.text.enabled = true;
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
                // infoTexts.First(t => t.key == "Durability").amountText.text = part.durability+"/"+part.partData.ingredients();
            }
        }

        public void OpenTooltip(GunData data)
        {
            
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
