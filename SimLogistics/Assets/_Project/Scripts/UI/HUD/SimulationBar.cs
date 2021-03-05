using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationBar : UIElement
{
    // Managers
    [SerializeField] private SimulationManager simulationManager;
    [SerializeField] private ConnectionManager connectionManager;

    // Buttons
    [SerializeField] private Button thinkButton;

    public void Show()
    {
        UpdateButtons();
        SetVisible(true);
    }

    public void ShowAnimate()
    {
        UpdateButtons();
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
    
    private void UpdateButtons()
    {
        if (ConnectionManager.IsAgentEndpointValid)
        {
            EnableThinkButton();
        }
        else
        {
            DisableThinkButton();
        }
    }

    // Button actions
    // --------------

    public void OnMovePressed()
    {
        print($"[{name}] OnMovePressed");
    }

    public void OnRepairPressed()
    {
        print($"[{name}] OnRepairPressed");
    }
    
    public void OnTransferPressed()
    {
        print($"[{name}] OnTransferPressed");
    }

    public void OnSimulatePressed()
    {
        print($"[{name}] OnSimulatePressed");
        simulationManager.Step(state => { });
    }

    public void OnThinkPressed()
    {
    }

    public void EnableThinkButton()
    {
        thinkButton.interactable = true;
    }

    public void DisableThinkButton()
    {
        thinkButton.interactable = false;
    }

}
