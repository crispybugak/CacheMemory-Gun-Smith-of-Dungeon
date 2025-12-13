using KBG.Item;
using UnityEngine;

namespace _06.SDW._01.Scripts.Item
{
    // 실제 인벤토리에 저장될 "완성된 파츠" 아이템
    // PartData(설계도)를 통해 만들어진 실제 아이템
    public class Part : IItem
    {
        [SerializeField] private PartData _partData;

        // IItem의 필수 구현 사항
        public override IItemData ItemData
        {
            get => _partData;
            set
            {
                if (value is PartData data)
                {
                    _partData = data;
                }
            }
        }

        // 제작 시 데이터를 받아와서 초기화하는 함수
        public void Initialize(PartData sourceData)
        {
            _partData = sourceData;

            // 나중에 내구도 등을 설정하려면 여기서 처리
            // name = sourceData.itemName + "(Instance)";
        }
    }
}