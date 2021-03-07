using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleItem : MonoBehaviour
{
    public UnityEvent onValueChanged = new UnityEvent();
    
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image toggleBackground;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private Color checkedColor;
    [SerializeField] private Color uncheckedColor;

    public bool IsOn
    {
        get => toggle.isOn;
        set => toggle.isOn = value;
    }

    public ToggleGroup Group
    {
        get => toggle.group;
        set => toggle.group = value;
    }
    
    public object data;

    private void Awake()
    {
        UpdateColor();
    }

    public string Label
    {
        get => labelText.text;
        set => labelText.text = value;
    }

    public void OnValueChange(bool value)
    {
        UpdateColor();
        onValueChanged.Invoke();
    }

    private void UpdateColor()
    {
        var newColor = IsOn ? checkedColor : uncheckedColor;
        toggleBackground.color = labelText.color = newColor;
    }
}