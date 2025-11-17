using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private Health _health;
    private Vignette _vignette;
    private Volume _volume;
    private float _maxIntensity = 0.175f;
    private float _currentInTensityValue;
    private float _minIntensity;
    private bool _isPlayed;
    private void Awake()
    {
        _volume = GetComponent<Volume>();
        _volume.profile.TryGet(out _vignette);   
    }
    private void Start()
    {
        _currentInTensityValue = _vignette.intensity.value;
        _currentInTensityValue = 0;
    }
    public void Play()
    {
        if (_isPlayed)
        {
            Debug.Log("isPlaying");
            return;
        }
        _isPlayed = true;
        StartCoroutine(EffectStartCT());
    }
    private IEnumerator EffectStartCT()
    {
        Debug.Log("beginPlaying");
        _currentInTensityValue = 0;
        if (_health._currentHealth > 0)
        {
            while (_vignette.intensity.value < _maxIntensity)
            {
                Debug.Log("whilePlaying");

                _vignette.intensity.value += 0.1f;

                yield return new WaitForSeconds(0.03f);
            }
            StartCoroutine(EffectBackCT());
        }
    }
    private IEnumerator EffectBackCT()
    {
        Debug.Log("beginEnding");

        _currentInTensityValue = _maxIntensity;
        while (_currentInTensityValue <= _maxIntensity)
        {
            Debug.Log("whileEnding");

            _currentInTensityValue = _vignette.intensity.value -= 0.1f;
            yield return new WaitForSeconds(0.1f);
            if (_currentInTensityValue < _minIntensity)
                break;
        }
        _isPlayed = false;
        Debug.Log("endPlaying");
    }
}
