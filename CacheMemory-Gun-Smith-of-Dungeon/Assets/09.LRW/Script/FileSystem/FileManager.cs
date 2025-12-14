using System.IO;
using UnityEngine;

namespace _1.Script.Lrw.FileSystem
{
    public static class FileManager
    {
        private static readonly string FolderName = "GameDataFolder";
        private static string _saveFolderPath = "Null";

        public static void PathReset()
        {
            CreatFolder();
        }
        private static void CreatFolder()
        {
            if(_saveFolderPath != "Null") return;
            
            _saveFolderPath = Path.Combine(Application.persistentDataPath, FolderName);
            _saveFolderPath = Path.GetFullPath(_saveFolderPath);
            if (!Directory.Exists(_saveFolderPath))
            {
                Directory.CreateDirectory(_saveFolderPath);
            }
        }
        public static void SetFile(string fileName,string gameData)
        {
            CreatFolder();
            string createFilePath = Path.GetFullPath(Path.Combine(_saveFolderPath, $"{fileName}.txt"));
            
            File.WriteAllText(createFilePath, gameData);
        }
        public static void SetFile<T>(string fileName, T gameData)
        {
            CreatFolder();
            string createFilePath = Path.GetFullPath(Path.Combine(_saveFolderPath, $"{fileName}.txt"));
            
            File.WriteAllText(createFilePath, JsonUtility.ToJson(gameData));
        }

        public static void AddFileData(string fileName,string addGameData)
        {
            CreatFolder();
            string createFilePath = Path.GetFullPath(Path.Combine(_saveFolderPath, $"{fileName}.txt"));
            
            File.AppendAllText(createFilePath, addGameData);
        }
        
        public static string ReadFile(string name)
        {
            CreatFolder();
            string path = Path.GetFullPath(Path.Combine(_saveFolderPath, $"{name}.txt"));
            if (!File.Exists(path))
            {
                return string.Empty;
            }
            return File.ReadAllText(path);
        }

        public static T ReadFile<T>(string name)
        {
            CreatFolder();
            string path = Path.GetFullPath(Path.Combine(_saveFolderPath, $"{name}.txt"));
            if (!File.Exists(path))
            {
                Debug.LogError("File not found");
                return default;
            }
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }
        
        
    }
}

