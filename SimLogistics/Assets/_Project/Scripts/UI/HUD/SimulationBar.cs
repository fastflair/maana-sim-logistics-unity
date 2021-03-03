using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationBar : UIElement
{
    public void Show()
    {
        print("Simulation Bar Show");
        SetVisible(true, Effect.Animate);
    }

    public void Hide()
    {
        print("Simulation Bar Hide");
        SetVisible(false, Effect.Animate);
    }
    
    // Connection state
    // ----------------
    
    public void OnConnectionReady()
    {
        print("OnConnectionReady");
    }

    public void OnConnectionNotReady()
    {
        print("OnConnectionNotReady");
    }

    public void OnConnectionError(string error)
    {
        print($"OnConnectionError: {error}");
    }
    
    // Simulation state
    // ----------------
    
    public void OnSimulationReady()
    {
        print("OnSimulationReady");
    }

    public void OnSimulationNotReady()
    {
        print("OnSimulationNotReady");
    }
   
    public void OnSimulationError(string error)
    {
        print($"OnSimulationError: {error}");
    }
}
