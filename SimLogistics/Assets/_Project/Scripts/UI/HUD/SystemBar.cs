using UnityEngine;
using UnityEngine.UI;

public class SystemBar : UIElement
{
    [SerializeField] private float initialShowDelay;
    
    // Managers
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private SimulationManager simulationManager;

    // Dialogs
    [SerializeField] private ConnectionsDialog connectionsDialogPrefab;
    [SerializeField] private NewSimulationDialog newSimulationDialogPrefab;
    [SerializeField] private LoadSimulationDialog loadSimulationDialogPrefab;

    // Buttons
    [SerializeField] private Button newSimulationButton;
    [SerializeField] private Button loadSimulationButton;
    [SerializeField] private Button deleteSimulationButton;
    [SerializeField] private Button simulationButton;

    public void ShowFirstTime()
    {
        print("System Bar ShowFirstTime");
        SetVisible(true, Effect.Animate).setDelay(initialShowDelay);
    }
    
    public void Show()
    {
        print("System Bar Show");
        SetVisible(true);
    }

    public void Hide()
    {
        print("System Bar Hide");
        SetVisible(false);
    }
    
    // Connection state
    // ----------------
    
    public void OnConnectionReady()
    {
        print("OnConnectionReady");
        EnableSimulationManagementButtons();
    }

    public void OnConnectionNotReady()
    {
        print("OnConnectionNotReady");
        DisableSimulationManagementButtons();
    }

    public void OnConnectionError(string error)
    {
        print($"SystemBar: OnConnectionError: {error}");
        DisableSimulationManagementButtons();
        SetVisible(true, Effect.Animate);
    }
    
    // Simulation state
    // ----------------
    
    public void OnSimulationReady()
    {
        print("OnSimulationReady");
        if(!simulationManager.IsDefaultCurrent) EnableDeleteSimulationButton();
        EnableSimulationButton();
    }

    public void OnSimulationNotReady()
    {
        print("OnSimulationNotReady");
        DisableDeleteSimulationButton();
        DisableSimulationButton();
    }
   
    public void OnSimulationError(string error)
    {
        print($"OnSimulationError: {error}");
    }

    public void EnableSimulationManagementButtons()
    {
        EnableNewSimulationButton();
        EnableLoadSimulationButton();
        if (simulationManager.IsDefaultCurrent)
            DisableDeleteSimulationButton();
        else
            EnableDeleteSimulationButton();
    }
    
    public void DisableSimulationManagementButtons()
    {
        DisableNewSimulationButton();
        DisableLoadSimulationButton();
        DisableDeleteSimulationButton();
    }
    
    // Connections
    // -----------

    public void OnConnections()
    {
        var dialog = uiManager.ShowDialogPrefab<ConnectionsDialog>(connectionsDialogPrefab);
        dialog.ConnectionManager = connectionManager;
    }

    // Load
    // ----

    public void OnLoadSimulation()
    {
        var dialog = uiManager.ShowDialogPrefab<LoadSimulationDialog>(loadSimulationDialogPrefab);
        // dialog.ConnectionManager = connectionManager;
    }

    public void EnableLoadSimulationButton()
    {
        loadSimulationButton.interactable = true;
    }

    public void DisableLoadSimulationButton()
    {
        loadSimulationButton.interactable = false;
    }

    // New
    // ---

    public void OnNewSimulation()
    {
        var dialog = uiManager.ShowDialogPrefab<NewSimulationDialog>(newSimulationDialogPrefab);
        dialog.UIManager = uiManager;
        dialog.SimulationManager = simulationManager;
    }

    public void EnableNewSimulationButton()
    {
        newSimulationButton.interactable = true;
    }

    public void DisableNewSimulationButton()
    {
        newSimulationButton.interactable = false;
    }

    // Delete
    // ------

    public void OnDeleteSimulation()
    {
        uiManager.ShowConfirmationDialog("Are you sure you want to delete this simulation?");
        // TODO: callbacks
    }
    
    public void EnableDeleteSimulationButton()
    {
        deleteSimulationButton.interactable = true;
    }

    public void DisableDeleteSimulationButton()
    {
        deleteSimulationButton.interactable = false;
    }

    // Simulation
    // ----------
    
    public void EnableSimulationButton()
    {
        simulationButton.interactable = true;
    }

    public void DisableSimulationButton()
    {
        simulationButton.interactable = false;
    }
    
    // Exit
    // ------
    
    public void OnExit()
    {
        Application.Quit();
    }
}