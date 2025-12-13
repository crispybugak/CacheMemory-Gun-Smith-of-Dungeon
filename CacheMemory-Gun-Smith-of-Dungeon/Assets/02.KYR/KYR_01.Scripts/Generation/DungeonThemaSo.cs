using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Theme")]
public class DungeonThemeSO : ScriptableObject
{
    [Header("Rooms")]
    public GameObject startRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject[] eventRoomPrefabs;
    public GameObject[] monsterRoomPrefabs;

    [Header("Corridors")]
    public GameObject horizontalCorridorPrefab;
    public GameObject verticalCorridorPrefab;
}

