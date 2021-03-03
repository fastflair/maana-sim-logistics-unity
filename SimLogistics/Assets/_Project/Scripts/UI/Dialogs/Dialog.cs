using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIElement))]
public class Dialog : MonoBehaviour
{
    public UIManager UIManager { get; set; }

    public LTDescr SetVisible(bool isVisible, UIElement.Effect effect)
    {
        var uiElement = GetComponent<UIElement>();
        return uiElement.SetVisible(isVisible, effect);
    }
    
    public void Hide()
    {
        UIManager.HideDialog(this);
    }
}
