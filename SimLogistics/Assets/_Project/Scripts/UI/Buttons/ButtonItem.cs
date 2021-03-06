using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonItem : Button
{
    private TMP_Text _buttonText;
    public object data;

    protected override void Awake()
    {
        _buttonText = GetComponentInChildren<TMP_Text>();
        base.Awake();
    }

    public string Label
    {
        get => _buttonText.text;
        set => _buttonText.text = value;
    }
}
