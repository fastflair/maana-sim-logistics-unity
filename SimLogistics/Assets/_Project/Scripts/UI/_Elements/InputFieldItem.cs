using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldItem: MonoBehaviour
{
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private TMP_InputField input;
    
    public string Label
    {
        get => labelText.text;
        set => labelText.text = value;
    }
    public string Placeholder
    {
        get => input.placeholder.GetComponent<TMP_Text>().text;
        set => input.placeholder.GetComponent<TMP_Text>().text = value;
    }
    public string Value
    {
        get => input.text;
        set => input.text = value;
    }
    public TMP_InputField Input => input;
}
