using UnityEngine;
using UnityEngine.SceneManagement;
using _06.SDW._01.Scripts.SO;
using Skill;

public class CharacterSelectButton : MonoBehaviour
{
    [Header("이 캐릭터의 설정 SO")]
    public HealthDataSO health;
    public AgentStaminaSO stamina;
    public PassiveSO passive;
    public SkillSO skill;
    public RuntimeAnimatorController animator;

    public string inGameSceneName = "JHJGameScene";

    public void OnClick()
    {
        PlayerSelectionSaveSystem.SaveSelection(health, stamina, passive, skill, animator);
        SceneManager.LoadScene(inGameSceneName);
    }
}