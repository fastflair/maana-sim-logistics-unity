using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Tab : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, IPointerClickHandler
{
    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;
    
    [SerializeField] private TabGroup tabGroup;

    public Image background;

    private void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabClick(this);
    }

    public void Selected()
    {
        onTabSelected.Invoke();
    }
    
    public void Deselected()
    {
        onTabDeselected.Invoke();
    }
}
