using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Serializer
{
    public static string SavePath => Path.Combine(Application.persistentDataPath, "saves");

    public static bool Save(string saveName, object saveData)
    {
        var formatter = GetBinaryFormatter();
        
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }

        var saveFilePath = Path.Combine(SavePath, $"{saveName}.save");

        using var file = File.Create(saveFilePath);
        formatter.Serialize(file, saveData);
        file.Close();
        
        return true;
    }

    public static object Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.Log($"Path does not exist: {path}");
            return null;
        }
        
        var formatter = GetBinaryFormatter();

        using var file = File.Open(path, FileMode.Open);

        try
        {
            var save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Failed to load file at {0}: {1}", path, e);
            return null;
        }
    }
    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter;
    }
}
