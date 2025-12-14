using System;
using _1.Script.Lrw.FileSystem;
using _1.Script.Lrw.FileSystem.Data;
using UnityEngine;
using UnityEngine.UI;

namespace _09.LRW.Script.Sound
{
    public class SoundSliderData : MonoBehaviour
    {
        [SerializeField] private Slider MasterVolumeSlider;
        [SerializeField] private Slider BGMVolumeSlider;
        [SerializeField] private Slider SFXVolumeSlider;

        private void Awake()
        {
            SoundSettingData a = FileManager.ReadFile<SoundSettingData>("SoundSettingData");;
            MasterVolumeSlider.value = a.masterVolume;
            BGMVolumeSlider.value = a.bgmVolume;
            SFXVolumeSlider.value = a.sfxVolume;
            AudioManager.Instance.SetVolume(new SoundSettingData(MasterVolumeSlider.value,BGMVolumeSlider.value,SFXVolumeSlider.value));
        }

        private void Update()
        {
            AudioManager.Instance.SetVolume(new SoundSettingData(MasterVolumeSlider.value,BGMVolumeSlider.value,SFXVolumeSlider.value));
        }

        public float GetVolume(SoundType type)
        {
            if (type == SoundType.BGM)
            {
                return BGMVolumeSlider.value;
            }

            if (type == SoundType.SFX)
            {
                return SFXVolumeSlider.value;
            }
            return MasterVolumeSlider.value;
        }

        private void OnDestroy()
        {
            FileManager.SetFile("SoundSettingData",new SoundSettingData(MasterVolumeSlider.value,BGMVolumeSlider.value,SFXVolumeSlider.value));
        }
    }
}