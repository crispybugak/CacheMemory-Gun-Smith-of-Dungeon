using DG.Tweening;
using Pathfinding.Ionic.Zip;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [Header("Effect")]
    public Volume _volume;

    [Header("Common Panel & Fader")]
    public Image fadePanel;

    [Header("A. Play/Character Select")]
    public Image Ria;
    public Image Rin;
    public Button playBtn;
    public Button playExitBtn;

    [Header("B. Settings Panel")]
    public Image optionPanel;
    public Button optionBtn;
    public Button optionExitBtn;

    [Header("D.Game Exit Panel")]
    public TextMeshProUGUI _text;
    public Image GameExitPanel;
    public Button GameExitBtn;
    public Button GameExitExitBtn;

    private float fadeDuration = 0.2f;

    private bool _isPanelOpen = false;
    private void Start()
    {
        _volume.enabled = false;

        InitializePanels(Ria);
        InitializePanels(Rin);
        InitializePanels(optionPanel);

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
        //panel.gameObject.SetActive(false);
    }

    //=============ON============//
    public void OnClickPlayButton()
    {
        OpenCharacterSelectPanel(Ria,Rin);
    }

    public void OnClickOptionButton()
    {
        OpenOptionPanel(optionPanel);
    }

    public void OnClicExitButton()
    {
        OpenExitPanel(GameExitPanel);
    }

    //===============OFF=============//
    public void OnClickPlayExitButton()
    {
        CloseCharacterSelectPanel(Ria,Rin);
    }

    public void OnClickOptionExitButton()
    {
        OnClickemptiness(optionPanel);
    }
    public void OnClickExitButtonExitButton()
    {
        OnClickemptiness(GameExitPanel);
        Debug.Log("허공이 눌림");
    }

    private void OpenCharacterSelectPanel(Image Ria, Image Rin)
    {
        if (Ria == null || Rin == null) return;

        Ria.gameObject.SetActive(true);
        Rin.gameObject.SetActive(true);

        Ria.raycastTarget = true;
        Rin.raycastTarget = true;

/*        Color riaColor = Ria.color; 
        riaColor.a = 0f; 
        Ria.color = riaColor;

        Color rinColor = Rin.color; 
        rinColor.a = 0f;
        Rin.color = rinColor;*/

        _volume.enabled = true;

        Sequence sequence = DOTween.Sequence();


        sequence.Append(Ria.DOFade(1, fadeDuration));
        sequence.Join(Rin.DOFade(1, fadeDuration));
    }

    private void CloseCharacterSelectPanel(Image Ria, Image Rin)
    {
        if (Ria == null || Rin == null) return;

        _volume.enabled = false;
        Ria.raycastTarget = false;
        Rin.raycastTarget = false;
        Sequence sequence = DOTween.Sequence();

        sequence.Append(Ria.DOFade(0, fadeDuration));
        sequence.Join(Rin.DOFade(0, fadeDuration));

        sequence.OnComplete(() =>
        {
            Ria.gameObject.SetActive(false);
            Rin.gameObject.SetActive(false);
        });
    }

    private void OpenExitPanel(Image panel)
    {
        if (panel == null) return;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.DOFade(1, 0.2f).OnComplete(() => panel.raycastTarget = true));
        sequence.Join(panel.GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.2f).OnComplete(() => panel.raycastTarget = true));
    }

    private void OnClickemptiness(Image panel)
    {
        if (panel == null) return;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.DOFade(0, 0.2f).OnComplete(() => panel.raycastTarget = false));
        sequence.Join(panel.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.2f).OnComplete(() => panel.raycastTarget = false));
    }
    public void OpenOptionPanel(Image panel)
    {
        if (panel == null) return;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.DOFade(1, 0.2f).OnComplete(() => panel.raycastTarget = true));
    }
    public void GameExit()
    {
        Application.Quit();
    }
}
