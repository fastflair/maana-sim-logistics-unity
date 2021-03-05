using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationBar : UIElement
{
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
}
