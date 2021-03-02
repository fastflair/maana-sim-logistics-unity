using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageDialog : Dialog
{
    [SerializeField] private TMP_Text textArea;

    public void SetText(string text)
    {
        textArea.text = text;
    }
    
    public void OnOkay()
    {
        Hide();
    }
}