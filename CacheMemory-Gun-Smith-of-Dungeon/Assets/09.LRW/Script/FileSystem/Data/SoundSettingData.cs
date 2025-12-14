using System;

namespace _1.Script.Lrw.FileSystem.Data
{
    [Serializable]
    public struct SoundSettingData
    {
        public SoundSettingData(float a,float b,float c)
        {
            MasterVolume = a;
            BGMVolume = b;
            SFXVolume = c;
        }
        public float MasterVolume;
        public float BGMVolume;
        public float SFXVolume;
    }
}