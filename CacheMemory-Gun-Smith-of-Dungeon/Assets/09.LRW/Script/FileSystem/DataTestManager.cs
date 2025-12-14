using _1.Script.Lrw.FileSystem.Data;
using UnityEngine;

namespace _1.Script.Lrw.FileSystem
{
    public class DataTestManager : MonoBehaviour
    {
        [SerializeField] private string fileName;
        [SerializeField] private string SaveText;
        [SerializeField] private SoundSettingData data;
        [ContextMenu("Save")]
        private void Save()
        {
            FileManager.SetFile(fileName,SaveText);
            
        }
        [ContextMenu("Add")]
        private void Add()
        {
            FileManager.AddFileData(fileName,SaveText);
            
        }

        [ContextMenu("Print")]
        private void Print()
        {
            Debug.Log(FileManager.ReadFile(fileName));
        }
        
        
        [ContextMenu("Save2")]
        private void Save2()
        {
            FileManager.SetFile(fileName,data);
            
        }

        
    }
}