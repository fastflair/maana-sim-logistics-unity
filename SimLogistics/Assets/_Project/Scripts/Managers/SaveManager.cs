using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static string SavePath => Path.Combine(Application.persistentDataPath, "saves");

    public static string SafeFileName(string fileName)
    {
        return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c, '-'));
    }
    
    public static string FullPath(string fileName)
    {
        var path = Path.Combine(SavePath, fileName) + ".save";
        print($"FullPath: {path}");
        return path;
    }

    public class SaveFile
    {
        public string FileName { get; }
        public DateTime LastWriteTime { get; private set; }

        public SaveFile(string fileName, DateTime lastWriteTime)
        {
            FileName = fileName;
            LastWriteTime = lastWriteTime;
        }
    }
    
    public List<SaveFile> saveFiles = new List<SaveFile>();
    public SaveFile CurrentSaveFile { get; private set; }

    private void Awake()
    {
        GetSaveFiles();
    }

    public bool Save(string fileName, object saveData)
    {
        var filePath = FullPath(fileName);
        if (!Serializer.Save(filePath, saveData)) return false;
        GetSaveFiles();
        return true;
    }

    public bool Delete(string fileName)
    {
        var filePath = FullPath(fileName);
        File.Delete(filePath);
        GetSaveFiles();
        return true;
    }

    public T Load<T>(string fileName) where T : class
    {
        var filePath = FullPath(fileName);
        var saveData = Serializer.Load(filePath);
        if (saveData == null) return null;
        File.SetLastWriteTime(filePath, DateTime.Now);
        GetSaveFiles();
        return saveData as T;
    }
    
    private void GetSaveFiles()
    {
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
            return;
        }

        saveFiles.Clear();
        
        var paths = Directory.GetFiles(SavePath);
        var latestWriteTime = DateTime.MinValue;
        foreach (var path in paths)
        {
            var lastWriteTime = File.GetLastWriteTime(path);
            var cleanName = Path.GetFileNameWithoutExtension(path);
            var saveFile = new SaveFile(cleanName, lastWriteTime);
            saveFiles.Add(saveFile);

            if (lastWriteTime <= latestWriteTime) continue;
            
            latestWriteTime = lastWriteTime;
            CurrentSaveFile = saveFile;
        }
    }
}
