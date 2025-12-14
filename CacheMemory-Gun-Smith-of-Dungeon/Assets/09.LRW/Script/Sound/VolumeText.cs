using System;
using _04.JHJ._01.Scripts.Manager;
using TMPro;
using UnityEngine;

namespace _09.LRW.Script.Sound
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class VolumeText : MonoBehaviour
    {
        [SerializeField] private SoundSliderData soundSliderData;
        [SerializeField] private string defultText;
        [SerializeField] private SoundType soundType;
        private TextMeshProUGUI _textMeshPro;
        private void Awake()
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
            
        }

        private void Update()
        {
            string a = ((int)(soundSliderData.GetVolume(soundType) * 100)).ToString();
            _textMeshPro.text = defultText + a;
        }
    }
}