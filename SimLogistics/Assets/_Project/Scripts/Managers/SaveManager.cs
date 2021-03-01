using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
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
        var safeFileName = Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '-'));
        if (!Serializer.Save(safeFileName, saveData)) return false;
        GetSaveFiles();
        return true;
    }

    public T Load<T>(string fileName) where T : class
    {
        var saveData = Serializer.Load(fileName);
        if (saveData == null) return null;
        var filePath = Path.Combine(Serializer.SavePath, fileName);
        File.SetLastWriteTime(filePath, DateTime.Now);
        GetSaveFiles();
        return saveData as T;
    }
    
    private void GetSaveFiles()
    {
        if (!Directory.Exists(Serializer.SavePath))
        {
            Directory.CreateDirectory(Serializer.SavePath);
        }

        saveFiles.Clear();
        
        var paths = Directory.GetFiles(Serializer.SavePath);
        var latestWriteTime = DateTime.MinValue;
        foreach (var path in paths)
        {
            var lastWriteTime = File.GetLastWriteTime(path);
            var saveFile = new SaveFile(path, lastWriteTime);
            saveFiles.Add(saveFile);

            if (lastWriteTime <= latestWriteTime) continue;
            
            latestWriteTime = lastWriteTime;
            CurrentSaveFile = saveFile;
        }
    }
}
