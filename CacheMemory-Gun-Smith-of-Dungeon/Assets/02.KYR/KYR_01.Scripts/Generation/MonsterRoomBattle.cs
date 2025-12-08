using System;
using UnityEngine;

public class MonsterRoomBattle : MonoBehaviour
{
   [SerializeField] private Collider2D enterTirgger;
   private RoomController roomController;
   
   private bool _cleared = false;
   private bool _entered = false;

   private void Awake()
   {
      roomController = GetComponent<RoomController>();
      
   }
   
   private void OnTriggerEnter2D(Collider2D other)
   {
      if (_entered || _cleared) return;
      if (!other.CompareTag("Player")) return;

      _entered = true;
      roomController.CloseAllLinkedDoors();   // 들어오면 문 닫기
      //몬스터 생성 시작
   }
   
   private void StartBattle(){}
   
   private void CearRoom(){}
   
}
