using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputDialog : Dialog
{
    public UnityEvent onCancel = new UnityEvent();
    public UnityEvent onOkay = new UnityEvent();

    [SerializeField] private InputFieldItem inputField;
    [SerializeField] private Button okayButton;
    
    public string Label
    {
        get => inputField.Label;
        set => inputField.Label = value;
    }
    
    public string Value
    {
        get => inputField.Value;
        set => inputField.Value = value;
    }
    
    public void OnCancel()
    {
        onCancel.Invoke();
        Hide();
    }

    public void OnOkay()
    {
        print("OnOkay");
        onOkay.Invoke();
        Hide();
    }
    
    private void UpdateButtons()
    {
        okayButton.interactable = inputField.Value != null;
    }
}
