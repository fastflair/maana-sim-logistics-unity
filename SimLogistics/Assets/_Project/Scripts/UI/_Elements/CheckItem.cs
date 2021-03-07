using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CheckItem : MonoBehaviour
{
    public UnityEvent onValueChanged = new UnityEvent();
    
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image toggleBackground;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private Color checkedColor;
    [SerializeField] private Color uncheckedColor;
    public bool IsChecked => toggle.isOn;

    public object data;

    public string Label
    {
        get => labelText.text;
        set => labelText.text = value;
    }

    public void OnValueChange(bool value)
    {
        var newColor = value ? checkedColor : uncheckedColor;
        toggleBackground.color =
            labelText.color = newColor;

        onValueChanged.Invoke();
    }
}