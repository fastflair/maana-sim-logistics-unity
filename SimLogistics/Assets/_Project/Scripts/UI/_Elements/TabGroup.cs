using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private Color tabIdle;
    [SerializeField] private Color tabHover;
    [SerializeField] private Color tabSelected;

    [SerializeField] private List<Tab> tabs;
    [SerializeField] private List<GameObject> pages;
    
    private Tab _selectedTab;

    private void Start()
    {
        OnTabClick(tabs[0]);
    }
    
    public void OnTabEnter(Tab button)
    {
        ResetTabs();
        if (_selectedTab == button) return;
        button.background.color = tabHover;
    }

    public void OnTabExit(Tab button)
    {
        ResetTabs();
    }

    public void OnTabClick(Tab button)
    {
        if (_selectedTab) _selectedTab.Deselected();
        
        _selectedTab = button;
        ResetTabs();
        button.background.color = tabSelected;

        var index = button.transform.GetSiblingIndex();
        for (var i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == index);
        }
        
        _selectedTab.Selected();
    }

    private void ResetTabs()
    {
        foreach (var button in tabs.Where(button => _selectedTab != button))
        {
            button.background.color = tabIdle;
        }
    }
}