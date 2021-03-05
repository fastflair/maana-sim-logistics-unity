using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationBar : UIElement
{
    public void Show()
    {
        SetVisible(true);
    }

    public void Hide()
    {
        SetVisible(false);
    }
    
    // Connection state
    // ----------------
    
    public void OnConnectionReady()
    {
    }

    public void OnConnectionNotReady()
    {
    }

    public void OnConnectionError(string error)
    {
    }
    
    // Simulation state
    // ----------------
    
    public void OnSimulationReady()
    {
    }

    public void OnSimulationNotReady()
    {
    }
   
    public void OnSimulationError(string error)
    {
    }
}
