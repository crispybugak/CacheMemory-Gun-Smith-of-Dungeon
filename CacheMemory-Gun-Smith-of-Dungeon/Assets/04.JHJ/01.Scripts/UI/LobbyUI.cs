using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [Header("Common Panel & Fader")]
    public Image fadePanel;
    [Header("A. Play/Stage Panel")]
    public Image stagePanel;
    public Button playBtn; 
    public Button playExitBtn;
    [Header("B. Settings Panel")]
    public Image settingsPanel;
    public Button settingsBtn; 
    public Button settingsExitBtn;
    [Header("C. Character Select Panel")]
    public Image charSelectPanel;
    public Button charSelectBtn;
    public Button charSelectExitBtn;
    [Header("D.Game Exit Panel")]
    public Image GameExitPanel;
    public Button GameExitBtn;
    public Button GameExitExitBtn;


    private float fadeDuration = 0.15f; 
    private void Start()
    {
        InitializePanels(stagePanel);
        InitializePanels(settingsPanel);
        InitializePanels(charSelectPanel);
        Color fadeColor = fadePanel.color;
        fadeColor.a = 0f;
        fadePanel.color = fadeColor;
    }
    private void InitializePanels(Image panel)
    {
        if (panel == null) return;

        Color panelColor = panel.color;
        panelColor.a = 0f;
        panel.color = panelColor;
        panel.gameObject.SetActive(false);
    }
    //=============ON============//
    public void OnClickPlayButton()
    {
        OpenPanel(stagePanel);
    }
    public void OnClickSettingsButton()
    {
        OpenPanel(settingsPanel);
    }
    public void OnClickCharSelectButton()
    {
        OpenPanel(charSelectPanel);
    }
    public void OnClicExitButton()
    {
        OpenPanel(GameExitPanel);
    }
    //===============OFF=============//
    public void OnClickPlayExitButton()
    {
        ClosePanel(stagePanel);
    }
    public void OnClickSettingsExitButton()
    {
        ClosePanel(settingsPanel);
    }
    public void OnClickCharSelectExitButton()
    {
        ClosePanel(charSelectPanel);
    }
    public void OnClickExitPanelExitButton()
    {
        ClosePanel(charSelectPanel);
    }
    private void OpenPanel(Image targetPanel)
    {
        if (targetPanel == null) return;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            fadePanel.DOFade(1, fadeDuration)
                .OnComplete(() => targetPanel.gameObject.SetActive(true))
        );

        sequence.Append(targetPanel.DOFade(1, fadeDuration));
        sequence.Append(fadePanel.DOFade(0, fadeDuration));
    }

    private void ClosePanel(Image targetPanel)
    {
        if (targetPanel == null) return;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(fadePanel.DOFade(1, fadeDuration));

        sequence.Append(targetPanel.DOFade(0, fadeDuration)
            .OnComplete(() => targetPanel.gameObject.SetActive(false))
        );
        sequence.Append(fadePanel.DOFade(0, fadeDuration));
    }
}