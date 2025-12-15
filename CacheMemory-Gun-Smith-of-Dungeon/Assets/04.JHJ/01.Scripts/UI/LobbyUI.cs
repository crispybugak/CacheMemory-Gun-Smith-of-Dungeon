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

    // ====== 배낭(로비 버튼 / 배낭 UI 루트들) ======
    [Header("C. Backpack (Open/Close)")]
    public Button bagBtn;
    public GameObject bagCanvasRoot;
    public GameObject bagExtraObject;

    // ====== 카테고리 바 별도 캔바스 ======
    [Header("C-0. Category Bar Root (Separate Canvas)")]
    public GameObject categoryBarRoot;

    // ====== 배낭 내부 카테고리 ======
    [Header("C-1. Backpack Category Bar")]
    public Button categoryBagBtn;
    public Button categoryCraftBtn;

    // ====== C-2: 이제 배낭 콘텐츠만 유지 ======
    [Header("C-2. Bag Content Canvas")]
    public GameObject bagContentCanvas;

    // ====== C-3: 제작(Create Content) 캔바스로 사용 ======
    [Header("C-3. Create Content Canvas (Craft)")]
    public GameObject createContentCanvas; // 기존 inventoryCanvasRoot 역할

    private float fadeDuration = 0.2f;

    // ====== 팝업(창) 열림 상태 잠금 ======
    private enum LobbyPopup { None, Character, Option, Exit, Backpack }
    private LobbyPopup _popup = LobbyPopup.None;

    // ====== 버튼 클릭 사운드 유틸 ======
    private void PlayClickSound()
    {
        if (AudioManager.Instance == null) return;
        //AudioManager.Instance.PlaySound("버튼클릭", 0.5f, 0.5f);
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

        // ★ fadePanel도 초기엔 투명 + 입력 차단 OFF
        if (fadePanel != null)
        {
            Color fadeColor = fadePanel.color;
            fadeColor.a = 0f;
            fadePanel.color = fadeColor;
            fadePanel.raycastTarget = false;
        }

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

        // ★ 핵심: 투명 상태에서는 입력을 먹지 않게
        panel.raycastTarget = false;

        // (선택) 처음부터 꺼두고 필요할 때 켜는 방식이면 더 안전
       // panel.gameObject.SetActive(false);
    }

    // ====== 배낭/카테고리바 오브젝트 초기 상태 세팅 ======
    private void InitializeBackpackObjects()
    {
        if (bagCanvasRoot)
        {
            bagCanvasRoot.SetActive(false);
            SetRaycastForObject(bagCanvasRoot, false);
        }

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

    // ====== 오브젝트 단위 레이캐스트 On/Off ======
    private void SetRaycastForObject(GameObject root, bool on)
    {
        if (root == null) return;
        
        // 1) CanvasGroup이 있으면 그것도 토글
        CanvasGroup cg = root.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = on;
            cg.interactable = on;
        }

        // 2) ★ CanvasGroup 유무와 관계없이 자식 Graphic들도 같이 토글
        Graphic[] graphics = root.GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].raycastTarget = on;
        }
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

    // ====== 배낭 버튼(로비) ======
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

    // 배낭 닫기용(허공 버튼에서 호출)
    public void OnClickBagExitButton()
    {
        if (_popup != LobbyPopup.Backpack) return;
        CloseBackpack();
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

    public void OpenOptionPanel(Image panel)
    {
        if (panel == null) return;

        panel.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.DOFade(1, 0.2f).OnComplete(() => panel.raycastTarget = true));
    }

    private void OnClickemptiness(Image panel)
    {
        if (panel == null) return;

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

    // ====== 배낭 열기/닫기 (카테고리바도 같이) ======
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

        // 기본은 "배낭(C-2)" 화면
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

    // ====== 카테고리 버튼 이벤트 ======
    // 배낭 버튼: C-2 ON, C-3 OFF
    private void OnClickCategoryBag()
    {
        if (_popup != LobbyPopup.Backpack) return;

        PlayClickSound();
        ShowBagView();
    }

    // 제작 버튼: C-3 ON, C-2 OFF
    private void OnClickCategoryCraft()
    {
        if (_popup != LobbyPopup.Backpack) return;

        PlayClickSound();
        ShowCreateContentView();
    }

    // ====== 표시 유틸 (뷰 전환) ======
    private void HideAllContentViews()
    {
        SetBagBaseVisible(false);
        SetCreateContentVisible(false);
    }

    private void ShowBagView()
    {
        // 요구사항: 배낭 버튼 누르면 C-2(배낭) 켜고, C-3 꺼짐
        SetCreateContentVisible(false);
        SetBagBaseVisible(true);
    }

    private void ShowCreateContentView()
    {
        // 요구사항: 제작 버튼 누르면 C-3 켜고, C-2 꺼짐
        SetBagBaseVisible(false);
        SetCreateContentVisible(true);
    }

    // ====== C-2 (Bag) ======
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

    // ====== C-3 (Create Content) ======
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
