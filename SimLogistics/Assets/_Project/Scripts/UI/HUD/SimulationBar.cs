using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationBar : UIElement
{
    [SerializeField] private SimulationManager simulationManager;
    
    public void Show()
    {
        SetVisible(true);
    }

    public void ShowAnimate()
    {
        SetVisible(true, Effect.Animate);
    }

    public void Hide()
    {
        SetVisible(false);
    }

    public void HideAnimate()
    {
        SetVisible(false, Effect.Animate);
    }
    
    public void OnReady()
    {
        if (!simulationManager.IsDefaultCurrent) ShowAnimate();
    }
}
