/*using System.Collections.Generic;
using _06.SDW._01.Scripts.Manager;
using KBG.Item;
using UnityEngine;

namespace _06.SDW._01.Scripts.Item
{
    public class CraftingTester : MonoBehaviour
    {
        [Header("Managers")]
        public CraftingManager craftingManager;

        [Header("Recipes (SO)")]
        public PartData Recipe; 

        [Header("Ingredients (SO)")]
        public Ingredient[] Items; 

        // 테스트용 가상 인벤토리
        private List<IItem> myInventory = new List<IItem>();

        /*private void Start()
        {
            if (Items != null && Items.Length > 0)
            {
                foreach (var ingredient in Items)
                {
                    // 각 재료마다 5개씩 추가 (테스트용)
                    for (int i = 0; i < 5; i++)
                    {
                        myInventory.Add(ingredient);
                    }
                }
            }
            
            Debug.Log($"[Test] 시작 인벤토리 수: {myInventory.Count}");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Craft();
            }
        }

        public void Craft()
        {
            Part result = craftingManager.TryCraft(Recipe, myInventory);

            if (result != null)
            {
                myInventory.Add(result);
                
                Debug.Log($"[Test] 제작 완료! 남은 인벤토리 수: {myInventory.Count}");
                Debug.Log($"[Test] 생성된 파츠: {result.ItemData.itemName}, 타입: {((PartData)result.ItemData).type}");
            }
        }*/
    }
}*/