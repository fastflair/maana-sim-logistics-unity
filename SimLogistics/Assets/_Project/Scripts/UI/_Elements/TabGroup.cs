using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private Color tabIdle;
    [SerializeField] private Color tabHover;
    [SerializeField] private Color tabSelected;

    [SerializeField] private List<GameObject> pages;
    
    private List<Tab> _tabs;
    private Tab _selectedTab;

    public void Subscribe(Tab button)
    {
        _tabs ??= new List<Tab>();
        if (_selectedTab == null) OnTabClick(button);

        _tabs.Add(button);
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
        foreach (var button in _tabs.Where(button => _selectedTab != button))
        {
            button.background.color = tabIdle;
        }
    }
}