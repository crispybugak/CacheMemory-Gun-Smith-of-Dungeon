using System;

namespace _1.Script.Lrw.FileSystem.Data
{
    [Serializable]
    public class SoundSettingData
    {
        public SoundSettingData(float a,float b,float c)
        {
            masterVolume = a;
            bgmVolume = b;
            sfxVolume = c;
        }
        public float masterVolume = 1f;
        public float bgmVolume = 1f;
        public float sfxVolume = 1f;
    }
}