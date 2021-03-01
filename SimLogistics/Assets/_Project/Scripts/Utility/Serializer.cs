using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Serializer
{
    public static bool Save(string filePath, object saveData)
    {
        var formatter = GetBinaryFormatter();
        using var file = File.Create(filePath);
        formatter.Serialize(file, saveData);
        file.Close();
        
        return true;
    }

    public static object Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.Log($"File path does not exist: {filePath}");
            return null;
        }
        
        var formatter = GetBinaryFormatter();

        using var file = File.Open(filePath, FileMode.Open);

        try
        {
            var save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Failed to load file at {0}: {1}", filePath, e);
            return null;
        }
    }
    
    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter;
    }
}
