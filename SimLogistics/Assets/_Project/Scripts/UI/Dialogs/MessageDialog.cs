using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageDialog : Dialog
{
    [SerializeField] private TMP_Text textArea;

    public string Text
    {
        get => textArea.text;
        set => textArea.text = value;
    }
    
    public virtual void OnOkay()
    {
        Destroy();
    }
}