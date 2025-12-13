using UnityEngine;
using System;
using UnityEngine;

public class MonsterRoomBattle : MonoBehaviour
{
   [SerializeField] private Collider2D enterTirgger;
   [SerializeField] private MonsterRoomSpawner spawner;
   private RoomController roomController;

   private bool _cleared = false;
   private bool _entered = false;

   private void Awake()
   {
      roomController = GetComponent<RoomController>();

      // 몬스터 방이 생성될 때 DungeonProgress에 등록
      if (DungeonProgress.Instance != null)
      {
         DungeonProgress.Instance.RegisterMonsterRoom(this);
      }
   }

   // Spawner가 모든 몬스터 처치 후 이 메서드를 호출해야 함
   public void ClearRoom()
   {
      if (_cleared) return;

      _cleared = true;
      roomController.OpenAllLinkedDoors(); // 문 다시 열기

      // DungeonProgress에 클리어 알림
      if (DungeonProgress.Instance != null)
      {
         DungeonProgress.Instance.MonsterRoomCleared(this);
      }

      Debug.Log($"[{gameObject.name}] 몬스터 방 클리어");
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (_entered || _cleared) return;
      if (!other.CompareTag("Player")) return;

      _entered = true;
      roomController.CloseAllLinkedDoors();
      spawner?.StartBattle(ClearRoom);
   }
}