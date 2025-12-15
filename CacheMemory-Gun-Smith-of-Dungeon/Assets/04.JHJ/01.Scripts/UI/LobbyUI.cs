using DG.Tweening;
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

    [Header("B. Settings Panel")]
    public Image optionPanel;
    public Button optionBtn;

    [Header("D.Game Exit Panel")]
    public Image GameExitPanel;
    public Button GameExitBtn;
    public Image GameExitYesBtn;

    [Header("C. Backpack (Open/Close)")]
    public Button bagBtn;
    public GameObject bagCanvasRoot;
    public GameObject bagExtraObject;

    [Header("C-0. Category Bar Root (Separate Canvas)")]
    public GameObject categoryBarRoot;

    [Header("C-1. Backpack Category Bar")]
    public Button categoryBagBtn;
    public Button categoryCraftBtn;

    [Header("C-2. Bag Content Canvas")]
    public GameObject bagContentCanvas;

    [Header("C-3. Create Content Canvas (Craft)")]
    public GameObject createContentCanvas;

    private float fadeDuration = 0.2f;

    public bool _bag;

    private enum LobbyPopup { None, Character, Option, Exit, Backpack }
    private LobbyPopup _popup = LobbyPopup.None;

    // ====== [추가] 옵션 패널 한번에 페이드용 CanvasGroup 캐시 ======
    private CanvasGroup _optionCg;

    private void PlayClickSound()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlaySound("버튼클릭", 0.5f, 0.5f);
    }

    private void SetMainButtonsInteractable(bool on)
    {
        if (playBtn) playBtn.interactable = on;
        if (optionBtn) optionBtn.interactable = on;
        if (GameExitBtn) GameExitBtn.interactable = on;
        if (bagBtn) bagBtn.interactable = on;
    }

    private void Start()
    {
        AudioManager.Instance.PlaySound("로비", 0.7f, 1f);
        _volume.enabled = false;

        InitializePanels(Ria);
        InitializePanels(Rin);
        InitializePanels(optionPanel);
        InitializePanels(GameExitPanel);
        InitializePanels(GameExitYesBtn);

        if (fadePanel != null)
        {
            Color fadeColor = fadePanel.color;
            fadeColor.a = 0f;
            fadePanel.color = fadeColor;
            fadePanel.raycastTarget = false;
        }

        // ====== [추가] 옵션 패널 CanvasGroup 준비 ======
        PrepareOptionCanvasGroup();

        InitializeBackpackObjects();
        BindCategoryButtons();

        SetMainButtonsInteractable(true);
        _popup = LobbyPopup.None;
    }

    private void InitializePanels(Image panel)
    {
        if (panel == null) return;

        Color panelColor = panel.color;
        panelColor.a = 0f;
        panel.color = panelColor;

        panel.raycastTarget = false;
    }

    private void InitializeBackpackObjects()
    {
        // if (bagCanvasRoot)
        // {
        //     bagCanvasRoot.SetActive(false);
        //     SetRaycastForObject(bagCanvasRoot, false);
        // }

        if (categoryBarRoot)
        {
            categoryBarRoot.SetActive(false);
            SetRaycastForObject(categoryBarRoot, false);
        }

        if (bagExtraObject)
        {
            bagExtraObject.SetActive(false);
            SetRaycastForObject(bagExtraObject, false);
        }

        if (bagContentCanvas)
        {
            bagContentCanvas.SetActive(false);
            SetRaycastForObject(bagContentCanvas, false);
        }

        if (createContentCanvas)
        {
            createContentCanvas.SetActive(false);
            SetRaycastForObject(createContentCanvas, false);
        }
    }

    private void SetRaycastForObject(GameObject root, bool on)
    {
        if (root == null) return;

        CanvasGroup cg = root.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = on;
            cg.interactable = on;
        }

        Graphic[] graphics = root.GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphics.Length; i++)
            graphics[i].raycastTarget = on;
    }

    private void BindCategoryButtons()
    {
        if (categoryBagBtn)
        {
            categoryBagBtn.onClick.RemoveListener(OnClickCategoryBag);
            categoryBagBtn.onClick.AddListener(OnClickCategoryBag);
        }

        if (categoryCraftBtn)
        {
            categoryCraftBtn.onClick.RemoveListener(OnClickCategoryCraft);
            categoryCraftBtn.onClick.AddListener(OnClickCategoryCraft);
        }
    }

    //=============ON============//
    public void OnClickPlayButton()
    {
        if (_popup != LobbyPopup.None) return;

        PlayClickSound();

        _popup = LobbyPopup.Character;
        SetMainButtonsInteractable(false);

        OpenCharacterSelectPanel(Ria, Rin);
    }

    public void OnClickOptionButton()
    {
        if (_popup != LobbyPopup.None) return;

        PlayClickSound();

        _popup = LobbyPopup.Option;
        SetMainButtonsInteractable(false);

        OpenOptionPanel(optionPanel);
    }

    public void OnClicExitButton()
    {
        if (_popup != LobbyPopup.None) return;

        PlayClickSound();

        _popup = LobbyPopup.Exit;
        SetMainButtonsInteractable(false);

        OpenExitPanel(GameExitPanel);
        OpenExitPanel(GameExitYesBtn);
    }

    public void OnClickBagButton()
    {
        if (_popup == LobbyPopup.Backpack)
        {
            PlayClickSound();
            CloseBackpack();
            return;
        }

        if (_popup != LobbyPopup.None) return;

        PlayClickSound();

        _popup = LobbyPopup.Backpack;
        SetMainButtonsInteractable(false);

        OpenBackpack();
        _bag = true;
    }

    //===============OFF=============//
    public void OnClickPlayExitButton()
    {
        CloseCharacterSelectPanel(Ria, Rin);
    }

    public void OnClickOptionExitButton()
    {
        if (_popup != LobbyPopup.Option) return;
        OnClickemptiness(optionPanel);
    }

    public void OnClickExitButtonExitButton()
    {
        if (_popup != LobbyPopup.Exit) return;

        OnClickemptiness(GameExitPanel);
        OnClickemptiness(GameExitYesBtn);
        Debug.Log("허공이 눌림");
    }

    public void OnClickBagExitButton()
    {
        if (_popup != LobbyPopup.Backpack) return;
        CloseBackpack();
        _bag = false;
    }

    private void OpenCharacterSelectPanel(Image ria, Image rin)
    {
        if (ria == null || rin == null) return;

        ria.gameObject.SetActive(true);
        rin.gameObject.SetActive(true);

        ria.raycastTarget = true;
        rin.raycastTarget = true;

        _volume.enabled = true;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(ria.DOFade(1, fadeDuration));
        sequence.Join(rin.DOFade(1, fadeDuration));
    }

    private void CloseCharacterSelectPanel(Image ria, Image rin)
    {
        if (ria == null || rin == null) return;

        _volume.enabled = false;
        ria.raycastTarget = false;
        rin.raycastTarget = false;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(ria.DOFade(0, fadeDuration));
        sequence.Join(rin.DOFade(0, fadeDuration));

        sequence.OnComplete(() =>
        {
            ria.gameObject.SetActive(false);
            rin.gameObject.SetActive(false);

            _popup = LobbyPopup.None;
            SetMainButtonsInteractable(true);
        });
    }

    private void OpenExitPanel(Image panel)
    {
        if (panel == null) return;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.DOFade(1, 0.2f).OnComplete(() => panel.raycastTarget = true));
        sequence.Join(panel.GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.2f).OnComplete(() => panel.raycastTarget = true));
    }

    // ====== [수정] 옵션은 CanvasGroup으로 한번에 페이드 ======
    public void OpenOptionPanel(Image panel)
    {
        if (panel == null) return;

        panel.gameObject.SetActive(true);

        // CanvasGroup 준비/캐시
        if (_optionCg == null) PrepareOptionCanvasGroup();

        panel.raycastTarget = true; // 기존 흐름 유지(허공 클릭 받을 때 필요하면)
        _optionCg.blocksRaycasts = true;
        _optionCg.interactable = true;

        _optionCg.DOKill();
        _optionCg.alpha = 0f;
        _optionCg.DOFade(1f, 0.2f);
    }

    // ====== [수정] 옵션만 CanvasGroup으로 닫고, 나머지는 기존대로 ======
    private void OnClickemptiness(Image panel)
    {
        if (panel == null) return;

        // 옵션 패널이면: 자식까지 한번에 페이드
        if (panel == optionPanel)
        {
            if (_optionCg == null) PrepareOptionCanvasGroup();

            panel.raycastTarget = false;
            _optionCg.blocksRaycasts = false;
            _optionCg.interactable = false;

            _optionCg.DOKill();
            _optionCg.DOFade(0f, 0.2f).OnComplete(() =>
            {
                panel.gameObject.SetActive(false);

                if (_popup == LobbyPopup.Option)
                {
                    _popup = LobbyPopup.None;
                    SetMainButtonsInteractable(true);
                }
            });

            return;
        }

        // 기존 로직 (Exit 등)
        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.DOFade(0, 0.2f).OnComplete(() => panel.raycastTarget = false));
        sequence.Join(panel.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.2f).OnComplete(() => panel.raycastTarget = false));

        sequence.OnComplete(() =>
        {
            panel.gameObject.SetActive(false);

            if (_popup == LobbyPopup.Option && panel == optionPanel)
            {
                _popup = LobbyPopup.None;
                SetMainButtonsInteractable(true);
            }
            else if (_popup == LobbyPopup.Exit && panel == GameExitYesBtn)
            {
                _popup = LobbyPopup.None;
                SetMainButtonsInteractable(true);
            }
        });
    }

    // ====== [추가] 옵션 CanvasGroup 준비 함수 ======
    private void PrepareOptionCanvasGroup()
    {
        if (optionPanel == null) return;

        _optionCg = optionPanel.GetComponent<CanvasGroup>();
        if (_optionCg == null) _optionCg = optionPanel.gameObject.AddComponent<CanvasGroup>();

        // 초기 상태는 기존 InitializePanels와 맞춰서 투명
        _optionCg.alpha = 0f;
        _optionCg.interactable = false;
        _optionCg.blocksRaycasts = false;
    }

    private void OpenBackpack()
    {
        Debug.Log("가방 열려버리기");

        if (bagCanvasRoot)
        {
            bagCanvasRoot.SetActive(true);
            SetRaycastForObject(bagCanvasRoot, true);
        }

        if (categoryBarRoot)
        {
            categoryBarRoot.SetActive(true);
            SetRaycastForObject(categoryBarRoot, true);
        }

        if (bagExtraObject)
        {
            bagExtraObject.SetActive(true);
            SetRaycastForObject(bagExtraObject, true);
        }

        ShowBagView();
    }

    private void CloseBackpack()
    {
        HideAllContentViews();

        if (categoryBarRoot)
        {
            SetRaycastForObject(categoryBarRoot, false);
            categoryBarRoot.SetActive(false);
        }

        if (bagCanvasRoot)
        {
            SetRaycastForObject(bagCanvasRoot, false);
            bagCanvasRoot.SetActive(false);
        }

        if (bagExtraObject)
        {
            SetRaycastForObject(bagExtraObject, false);
            bagExtraObject.SetActive(false);
        }

        _popup = LobbyPopup.None;
        SetMainButtonsInteractable(true);
    }

    private void OnClickCategoryBag()
    {
        if (_popup != LobbyPopup.Backpack) return;

        PlayClickSound();
        ShowBagView();
    }

    private void OnClickCategoryCraft()
    {
        if (_popup != LobbyPopup.Backpack) return;

        PlayClickSound();
        ShowCreateContentView();
    }

    private void HideAllContentViews()
    {
        SetBagBaseVisible(false);
        SetCreateContentVisible(false);
    }

    private void ShowBagView()
    {
        SetCreateContentVisible(false);
        SetBagBaseVisible(true);
    }

    private void ShowCreateContentView()
    {
        SetBagBaseVisible(false);
        SetCreateContentVisible(true);
    }

    private void SetBagBaseVisible(bool on)
    {
        if (!bagContentCanvas) return;

        if (on)
        {
            bagContentCanvas.SetActive(true);
            SetRaycastForObject(bagContentCanvas, true);
        }
        else
        {
            SetRaycastForObject(bagContentCanvas, false);
            bagContentCanvas.SetActive(false);
        }
    }

    private void SetCreateContentVisible(bool on)
    {
        if (!createContentCanvas) return;

        if (on)
        {
            createContentCanvas.SetActive(true);
            SetRaycastForObject(createContentCanvas, true);
        }
        else
        {
            SetRaycastForObject(createContentCanvas, false);
            createContentCanvas.SetActive(false);
        }
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
