using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PropertyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private TMP_Text valueText;

    public string Label
    {
        get => labelText.text;
        set => labelText.text = value;
    }
    public string Value
    {
        get => valueText.text;
        set => valueText.text = value;
    }
}
