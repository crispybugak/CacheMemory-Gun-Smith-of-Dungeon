using System.IO;
using UnityEngine;
using _06.SDW._01.Scripts.SO;
using Skill;

public static class PlayerStatSaveManager
{
    private const string FileName = "player_stats.json";

    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, FileName);

    public static PlayerStatSaveData LastLoadedData { get; private set; }

    public static void Save(
        HealthDataSO healthSO,
        AgentStaminaSO staminaSO,
        PassiveSO passiveSO,
        SkillSO skillSO,
        RuntimeAnimatorController animatorController
    )
    {
        PlayerStatSaveData data = new PlayerStatSaveData
        {
            health = healthSO.ToSaveData(),
            stamina = staminaSO.ToSaveData(),
            passiveName = passiveSO != null ? passiveSO.name : null,
            skillName   = skillSO   != null ? skillSO.name   : null,
            animatorName = animatorController != null ? animatorController.name : null
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

#if UNITY_EDITOR
        Debug.Log($"Saved Player Stats: {SavePath}\n{json}");
#endif
    }

    public static PlayerStatSaveData Load(
        HealthDataSO healthSO,
        AgentStaminaSO staminaSO,
        Health health,
        Stamina stamina
    )
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No player stat save file found.");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        PlayerStatSaveData data = JsonUtility.FromJson<PlayerStatSaveData>(json);

        if (data.health != null)
            healthSO.ApplySaveData(data.health);
        if (data.stamina != null)
            staminaSO.ApplySaveData(data.stamina);

        // Health / Stamina 컴포넌트에 반영
        health?.OnDamaged(0); // or health.InitFromSO() if 네가 만들어뒀으면 그걸로
        stamina?.AddBonusMaxStamina(0); // 또는 stamina.InitFromSO()

        LastLoadedData = data;

#if UNITY_EDITOR
        Debug.Log($"Loaded Player Stats from {SavePath}\n{json}");
#endif

        return data;
    }
}
