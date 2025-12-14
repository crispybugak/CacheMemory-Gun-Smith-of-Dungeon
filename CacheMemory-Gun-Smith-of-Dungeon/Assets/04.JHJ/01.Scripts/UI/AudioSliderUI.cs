using UnityEngine;
using UnityEngine.UI;

public class AudioSliderUI : MonoBehaviour
{
    [Header("Sliders (0~1)")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Optional: Show Value Text (0~100)")]
    [SerializeField] private Text masterValueText;
    [SerializeField] private Text bgmValueText;
    [SerializeField] private Text sfxValueText;

    private AudioManager _audio;

    private void Awake()
    {
        _audio = AudioManager.Instance;
    }

    private void OnEnable()
    {
        _audio = AudioManager.Instance;

        // 슬라이더 기본 세팅
        SetupSlider(masterSlider);
        SetupSlider(bgmSlider);
        SetupSlider(sfxSlider);

        // 초기 값 반영(현재 AudioManager 값 -> 슬라이더)
        SyncFromAudioManager();

        // 리스너 연결
        if (masterSlider) masterSlider.onValueChanged.AddListener(OnMasterChanged);
        if (bgmSlider) bgmSlider.onValueChanged.AddListener(OnBgmChanged);
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(OnSfxChanged);
    }

    private void OnDisable()
    {
        // 리스너 해제(중복 등록 방지)
        if (masterSlider) masterSlider.onValueChanged.RemoveListener(OnMasterChanged);
        if (bgmSlider) bgmSlider.onValueChanged.RemoveListener(OnBgmChanged);
        if (sfxSlider) sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
    }

    private void SetupSlider(Slider s)
    {
        if (!s) return;
        s.minValue = 0f;
        s.maxValue = 1f;
        s.wholeNumbers = false;
    }

    private void SyncFromAudioManager()
    {
        if (_audio == null) return;

        // AudioManager의 현재 볼륨 값을 슬라이더에 반영
        if (masterSlider) masterSlider.SetValueWithoutNotify(_audio.MasterVolume);
        if (bgmSlider) bgmSlider.SetValueWithoutNotify(_audio.BGMVolume);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(_audio.SFXVolume);

        UpdateValueTexts();
    }

    private void OnMasterChanged(float value)
    {
        if (_audio == null) return;
        _audio.VolumeChanger(value, SoundType.Master);
        UpdateValueTexts();
    }

    private void OnBgmChanged(float value)
    {
        if (_audio == null) return;
        _audio.VolumeChanger(value, SoundType.BGM);
        UpdateValueTexts();
    }

    private void OnSfxChanged(float value)
    {
        if (_audio == null) return;
        _audio.VolumeChanger(value, SoundType.SFX);
        UpdateValueTexts();
    }

    private void UpdateValueTexts()
    {
        // 퍼센트 표기(선택)
        if (masterValueText && masterSlider) masterValueText.text = Mathf.RoundToInt(masterSlider.value * 100f).ToString();
        if (bgmValueText && bgmSlider) bgmValueText.text = Mathf.RoundToInt(bgmSlider.value * 100f).ToString();
        if (sfxValueText && sfxSlider) sfxValueText.text = Mathf.RoundToInt(sfxSlider.value * 100f).ToString();
    }
}
