using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public TMP_InputField saveName;
    public GameObject loadButton;

    public void OnSave()
    {
    }

    public string[] saveFiles;

    public void GetSaveFiles()
    {
        if (!Directory.Exists(Serializer.SavePath))
        {
            Directory.CreateDirectory(Serializer.SavePath);
        }

        saveFiles = Directory.GetFiles(Serializer.SavePath);
    }
}
