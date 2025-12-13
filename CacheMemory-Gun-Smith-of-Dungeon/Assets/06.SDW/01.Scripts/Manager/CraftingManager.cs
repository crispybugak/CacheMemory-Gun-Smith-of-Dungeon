using System.Collections.Generic;
using KBG.Item;
using UnityEngine;
using Part = _06.SDW._01.Scripts.Item.Part;

namespace _06.SDW._01.Scripts.Manager
{
    public class CraftingManager : MonoBehaviour
    {
        public Part TryCraft(PartData recipe, List<IItem> inventory)
        {
            // 1. 재료 검사 (소모할 아이템들을 미리 찾음)
            if (!CheckIngredients(recipe, inventory, out List<IItem> itemsToConsume))
            {
                Debug.Log($"[Crafting] 제작 실패: 재료가 부족합니다. (목표: {recipe.itemName})");
                return null;
            }

            // 2. 재료 소모 (인벤토리에서 제거)
            foreach (var item in itemsToConsume)
            {
                inventory.Remove(item);
                // 필요하다면 아이템 파괴 로직 추가 (예: Destroy(item) if it's an instance)
            }
            Debug.Log($"[Crafting] 재료 소모 완료: {itemsToConsume.Count}개");

            // 3. 결과물 생성 (ScriptableObject 인스턴스화)
            // 주의: new Part()가 아니라 CreateInstance를 써야 합니다.
            Part newPart = ScriptableObject.CreateInstance<Part>();
            
            // 데이터 주입
            newPart.Initialize(recipe);

            Debug.Log($"[Crafting] 제작 성공! 획득 아이템: {newPart.ItemData.itemName}");
            return newPart;
        }

        private bool CheckIngredients(PartData recipe, List<IItem> inventory, out List<IItem> itemsToConsume)
        {
            itemsToConsume = new List<IItem>();

            // 레시피에 정의된 모든 필요 재료 순회
            foreach (var req in recipe.ingredients)
            {
                IngredientType typeNeeded = req.requiredIngredient;
                int amountNeeded = req.requiredAmount;
                int currentAmount = 0;

                foreach (var item in inventory)
                {
                    // 이제 Ingredient가 IItem을 상속받았으므로 이 코드가 정상 작동합니다.
                    if (item is Ingredient ingItem) 
                    {
                        // 타입 비교 (Flags)
                        if (ingItem.type.HasFlag(typeNeeded))
                        {
                            if (!itemsToConsume.Contains(ingItem))
                            {
                                itemsToConsume.Add(ingItem);
                                currentAmount++;
                                if (currentAmount >= amountNeeded) break;
                            }
                        }
                    }
                }

                // 해당 재료의 수량이 부족하면 즉시 실패 처리
                if (currentAmount < amountNeeded)
                {
                    Debug.LogWarning($"[Crafting] 재료 부족: {typeNeeded} (필요: {amountNeeded}, 보유: {currentAmount})");
                    return false;
                }
            }

            // 모든 재료 충족됨
            return true;
        }
    }
}