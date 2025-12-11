using System;
using System.IO;
using UnityEngine;
using _06.SDW._01.Scripts.SO;
using Skill;

public static class PlayerSelectionSaveSystem
{
    [Serializable]
    public class PlayerSelectionSaveData
    {
        public HealthDataSO.SaveData health;
        public AgentStaminaSO.SaveData stamina;

        public string passiveName;
        public string skillName;
        public string animatorName;
    }

    private const string FileName = "playerSelection.json";
    private static string FullPath => Path.Combine(Application.persistentDataPath, FileName);

    // === 캐릭터 선택 씬에서 호출 (버튼 OnClick 등) ===
    public static void SaveSelection(
        HealthDataSO healthSO,
        AgentStaminaSO staminaSO,
        PassiveSO passive,
        SkillSO skill,
        RuntimeAnimatorController animator)
    {
        if (healthSO == null || staminaSO == null)
        {
            Debug.LogError("[PlayerSelectionSaveSystem] healthSO 또는 staminaSO가 null 입니다.");
            return;
        }

        PlayerSelectionSaveData data = new PlayerSelectionSaveData
        {
            health = healthSO.ToSaveData(),
            stamina = staminaSO.ToSaveData(),
            passiveName = passive != null ? passive.name : string.Empty,
            skillName = skill != null ? skill.name : string.Empty,
            animatorName = animator != null ? animator.name : string.Empty
        };

        string json = JsonUtility.ToJson(data, true);

        try
        {
            File.WriteAllText(FullPath, json);
#if UNITY_EDITOR
            Debug.Log($"[PlayerSelectionSaveSystem] 저장 완료: {FullPath}\n{json}");
#endif
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlayerSelectionSaveSystem] 저장 실패: {e}");
        }
    }

    // === 인게임에서 불러올 때 사용 ===
    public static bool TryLoad(out PlayerSelectionSaveData data)
    {
        data = null;

        if (!File.Exists(FullPath))
        {
            Debug.LogWarning("[PlayerSelectionSaveSystem] 저장 파일이 없습니다.");
            return false;
        }

        try
        {
            string json = File.ReadAllText(FullPath);
            data = JsonUtility.FromJson<PlayerSelectionSaveData>(json);
            return data != null;
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlayerSelectionSaveSystem] 로드 실패: {e}");
            return false;
        }
    }
}
