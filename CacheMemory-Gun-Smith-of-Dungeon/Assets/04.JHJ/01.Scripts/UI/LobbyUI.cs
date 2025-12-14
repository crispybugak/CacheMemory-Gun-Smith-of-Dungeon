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

    [Header("C-2. Content Canvases")]
    public GameObject bagContentCanvas;
    public GameObject craftContentCanvas;

    // ====== NEW: C-3 인벤토리 캔바스(UI) ======
    [Header("C-3. Inventory Canvas")]
    public GameObject inventoryCanvasRoot;

    private float fadeDuration = 0.2f;

    private bool _isPanelOpen = false;

    // ====== 팝업(창) 열림 상태 잠금 ======
    private enum LobbyPopup { None, Character, Option, Exit, Backpack }
    private LobbyPopup _popup = LobbyPopup.None;

    // ====== 버튼 클릭 사운드 유틸 ======
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

        if (bagContentCanvas)
        {
            bagContentCanvas.SetActive(false);
            SetRaycastForObject(bagContentCanvas, false);
        }

        if (craftContentCanvas)
        {
            craftContentCanvas.SetActive(false);
            SetRaycastForObject(craftContentCanvas, false);
        }

        if (inventoryCanvasRoot)
        {
            inventoryCanvasRoot.SetActive(false);
            SetRaycastForObject(inventoryCanvasRoot, false);
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
        //PlayClickSound();
        CloseCharacterSelectPanel(Ria, Rin);
    }

    public void OnClickOptionExitButton()
    {
        // ===== 핵심 변경: 옵션 창이 떠있을 때만 허공 클릭이 먹음 =====
        if (_popup != LobbyPopup.Option) return;

        // 허공: 사운드 없음
        OnClickemptiness(optionPanel);
    }

    public void OnClickExitButtonExitButton()
    {
        // ===== 핵심 변경: 종료 창이 떠있을 때만 허공 클릭이 먹음 =====
        if (_popup != LobbyPopup.Exit) return;

        // 허공: 사운드 없음
        OnClickemptiness(GameExitPanel);
        OnClickemptiness(GameExitYesBtn);
        Debug.Log("허공이 눌림");
    }

    // 배낭 닫기용(허공 버튼에서 호출)
    public void OnClickBagExitButton()
    {
        // ===== 핵심 변경: 배낭이 열려있을 때만 허공 클릭이 먹음 =====
        if (_popup != LobbyPopup.Backpack) return;

        // 허공: 사운드 없음
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

        // 기본은 가방 화면
        ShowBagView();
    }

    private void CloseBackpack()
    {
        // 전부 끄기
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
    // 제작 버튼: C-2(기존 컨텐츠) 전부 끄고, C-3(InventoryCanvas)만 켜기
    private void OnClickCategoryCraft()
    {
        if (_popup != LobbyPopup.Backpack) return;

        PlayClickSound();
        ShowInventoryView();
    }

    // 가방 버튼: C-3 끄고, C-2의 bagContentCanvas만 켜기
    private void OnClickCategoryBag()
    {
        if (_popup != LobbyPopup.Backpack) return;

        PlayClickSound();
        ShowBagView();
    }

    // ====== 표시 유틸 (뷰 전환) ======
    private void HideAllContentViews()
    {
        // C-2 OFF
        SetBagBaseVisible(false);
        SetCraftVisible(false);

        // C-3 OFF
        SetInventoryVisible(false);
    }

    private void ShowBagView()
    {
        // 제작/인벤토리 쪽 모두 끄고 가방만 켬
        SetInventoryVisible(false);
        SetCraftVisible(false);
        SetBagBaseVisible(true);
    }

    private void ShowInventoryView()
    {
        // 요구사항: 제작 누르면 C-2에 할당된 UI들 끄고(C-2 전체 OFF), C-3 인벤토리 캔바스 ON
        SetBagBaseVisible(false);
        SetCraftVisible(false);
        SetInventoryVisible(true);
    }

    // ====== 기존 C-2 ======
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

    // ====== NEW C-3 ======
    private void SetInventoryVisible(bool on)
    {
        if (!inventoryCanvasRoot) return;

        if (on)
        {
            inventoryCanvasRoot.SetActive(true);
            SetRaycastForObject(inventoryCanvasRoot, true);
        }
        else
        {
            SetRaycastForObject(inventoryCanvasRoot, false);
            inventoryCanvasRoot.SetActive(false);
        }
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
