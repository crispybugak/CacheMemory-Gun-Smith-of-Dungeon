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
    public Button bagBtn;                 // 로비의 배낭 버튼
    public GameObject bagCanvasRoot;      // 배낭 UI 전체 루트(내용물 영역 등)
    public GameObject bagExtraObject;     // 배낭 열 때 같이 켤 오브젝트(있으면)

    // ====== 카테고리 바 별도 캔바스 ======
    [Header("C-0. Category Bar Root (Separate Canvas)")]
    public GameObject categoryBarRoot;    // 카테고리 바 캔바스/루트 오브젝트

    // ====== 배낭 내부 카테고리 ======
    [Header("C-1. Backpack Category Bar")]
    public Button categoryBagBtn;         // 카테고리바의 "Bag" 버튼
    public Button categoryCraftBtn;       // 카테고리바의 "Create" 버튼

    [Header("C-2. Content Canvases")]
    public GameObject bagContentCanvas;   // 배낭 내용물 캔바스/패널(항상 기본으로 켜둘 대상)
    public GameObject craftContentCanvas; // 제작 내용물 캔바스/패널(오버레이처럼 켰다/껐다)

    private float fadeDuration = 0.2f;

    private bool _isPanelOpen = false;

    // ====== 팝업(창) 열림 상태 잠금 ======
    private enum LobbyPopup { None, Character, Option, Exit, Backpack }
    private LobbyPopup _popup = LobbyPopup.None;

    private void SetMainButtonsInteractable(bool on)
    {
        if (playBtn) playBtn.interactable = on;
        if (optionBtn) optionBtn.interactable = on;
        if (GameExitBtn) GameExitBtn.interactable = on;
        if (bagBtn) bagBtn.interactable = on;
    }

    private void Start()
    {
        _volume.enabled = false;

        InitializePanels(Ria);
        InitializePanels(Rin);
        InitializePanels(optionPanel);

        Color fadeColor = fadePanel.color;
        fadeColor.a = 0f;
        fadePanel.color = fadeColor;

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

        // 배낭 콘텐츠는 "배낭 UI가 열릴 때" 켜줄 것이므로 여기서는 꺼둠
        if (bagContentCanvas)
        {
            bagContentCanvas.SetActive(false);
            SetRaycastForObject(bagContentCanvas, false);
        }

        // 제작 콘텐츠도 꺼둠
        if (craftContentCanvas)
        {
            craftContentCanvas.SetActive(false);
            SetRaycastForObject(craftContentCanvas, false);
        }
    }

    // ====== 오브젝트 단위 레이캐스트 On/Off ======
    private void SetRaycastForObject(GameObject root, bool on)
    {
        if (root == null) return;

        CanvasGroup cg = root.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = on;
            cg.interactable = on;
            return;
        }

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
        _popup = LobbyPopup.Character;
        SetMainButtonsInteractable(false);

        OpenCharacterSelectPanel(Ria, Rin);
    }

    public void OnClickOptionButton()
    {
        if (_popup != LobbyPopup.None) return;
        _popup = LobbyPopup.Option;
        SetMainButtonsInteractable(false);

        OpenOptionPanel(optionPanel);
    }

    public void OnClicExitButton()
    {
        if (_popup != LobbyPopup.None) return;
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
            CloseBackpack();
            return;
        }

        if (_popup != LobbyPopup.None) return;

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
        OnClickemptiness(optionPanel);
    }

    public void OnClickExitButtonExitButton()
    {
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

    private void OpenCharacterSelectPanel(Image Ria, Image Rin)
    {
        if (Ria == null || Rin == null) return;

        Ria.gameObject.SetActive(true);
        Rin.gameObject.SetActive(true);

        Ria.raycastTarget = true;
        Rin.raycastTarget = true;

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

    private void OnClickemptiness(Image panel)
    {
        if (panel == null) return;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.DOFade(0, 0.2f).OnComplete(() => panel.raycastTarget = false));
        sequence.Join(panel.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.2f).OnComplete(() => panel.raycastTarget = false));

        sequence.OnComplete(() =>
        {
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

    public void OpenOptionPanel(Image panel)
    {
        if (panel == null) return;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(panel.DOFade(1, 0.2f).OnComplete(() => panel.raycastTarget = true));
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

        // ====== 변경: 배낭은 기본으로 켠다(제작은 기본 OFF) ======
        SetBagBaseVisible(true);
        SetCraftVisible(false);
    }

    private void CloseBackpack()
    {
        // 내부 콘텐츠 정리
        SetCraftVisible(false);
        SetBagBaseVisible(false);

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
    // 요구사항:
    // - Craft 버튼: Craft만 켠다(배낭은 끄지 않는다)
    // - Bag 버튼: Craft만 끈다(배낭은 그대로)
    private void OnClickCategoryCraft()
    {
        if (_popup != LobbyPopup.Backpack) return;
        SetCraftVisible(true);
    }

    private void OnClickCategoryBag()
    {
        if (_popup != LobbyPopup.Backpack) return;
        SetCraftVisible(false);
    }

    // ====== 변경된 표시 유틸 ======
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

    private void SetCraftVisible(bool on)
    {
        if (!craftContentCanvas) return;

        if (on)
        {
            craftContentCanvas.SetActive(true);
            SetRaycastForObject(craftContentCanvas, true);
        }
        else
        {
            SetRaycastForObject(craftContentCanvas, false);
            craftContentCanvas.SetActive(false);
        }
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
