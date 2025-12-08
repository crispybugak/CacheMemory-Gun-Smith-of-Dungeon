using UnityEngine;
using System;

public class DungeonProgress : MonoBehaviour
{
    public static DungeonProgress Instance { get; private set; }

    private int _totalMonsterRooms;
    private int _clearedMonsterRooms;

    public event Action OnAllMonsterRoomsCleared;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterMonsterRoom(MonsterRoomBattle room)
    {
        _totalMonsterRooms++;
    }

    public void MonsterRoomCleared(MonsterRoomBattle room)
    {
        _clearedMonsterRooms++;
        if (_clearedMonsterRooms >= _totalMonsterRooms)
        {
            OnAllMonsterRoomsCleared?.Invoke();
        }
    }
}
