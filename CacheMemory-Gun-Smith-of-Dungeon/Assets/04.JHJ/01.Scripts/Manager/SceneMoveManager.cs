using System;
using System.IO;
using _06.SDW._01.Scripts.SO;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMoveManager : MonoSingleton<SceneMoveManager>
{
    [Header("이 버튼이 대표하는 캐릭터 스탯 SO")]
    [SerializeField] private HealthDataSO healthPresetSO;
    [SerializeField] private AgentStaminaSO staminaPresetSO;

    [Header("이 캐릭터의 패시브 / 스킬 / 애니메이션")]
    [SerializeField] private PassiveSO passiveSO;
    [SerializeField] private SkillSO skillSO;
    [SerializeField] private RuntimeAnimatorController animator;

    private void Awake()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClickSelect);
        }
    }

    public void OnClickSelect()
    {
        PlayerStatSaveManager.Save(
            healthPresetSO,
            staminaPresetSO,
            passiveSO,
            skillSO,
            animator
        );
        
        MoveToGameScene();
    }
    
    private void MoveToGameScene()
    {
        SceneManager.LoadScene("JHJGameScene");
    }
}
