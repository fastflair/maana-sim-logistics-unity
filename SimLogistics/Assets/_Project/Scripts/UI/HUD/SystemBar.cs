using UnityEngine;
using UnityEngine.UI;

public class SystemBar : UIElement
{
    [SerializeField] private float initialShowDelay;
    private bool _hasShown;
    
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
        print($"[{name}] ShowFirstTime: _hasShown={_hasShown}");
        if (_hasShown) return;
        _hasShown = true;
        SetVisible(true, Effect.Animate).setDelay(initialShowDelay);
    }
    
    public void Show()
    {
        print($"[{name}] Show: _hasShown={_hasShown}");
        if (!_hasShown) return;
        
        SetVisible(true);
    }

    public void Hide()
    {
        print($"[{name}] Hide");
        SetVisible(false);
    }
    
    // Connection state
    // ----------------
    
    public void OnConnectionReady()
    {
        print($"[{name}] OnConnectionReady");
        EnableSimulationManagementButtons();
    }

    public void OnConnectionNotReady()
    {
        print($"[{name}] OnConnectionNotReady");
        DisableSimulationManagementButtons();
    }

    public void OnConnectionError(string error)
    {
        print($"[{name}] OnConnectionError");
        DisableSimulationManagementButtons();
        SetVisible(true, Effect.Animate);
    }
    
    // Simulation state
    // ----------------
    
    public void OnDefaultSimulation()
    {
        print($"[{name}] OnDefaultSimulation");
        EnableSimulationManagementButtons();
        DisableSimulationButton();
        Show();
    }
    
    public void OnNewSimulation()
    {
        print($"[{name}] OnNewSimulation");
        EnableSimulationManagementButtons();
        EnableSimulationButton();
        Hide();
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

    public void OnConnectionsPressed()
    {
        var dialog = uiManager.ShowDialogPrefab<ConnectionsDialog>(connectionsDialogPrefab);
        dialog.ConnectionManager = connectionManager;
    }

    // Load
    // ----

    public void OnLoadSimulationPressed()
    {
        var dialog = uiManager.ShowDialogPrefab<LoadSimulationDialog>(loadSimulationDialogPrefab);
        dialog.ConnectionManager = connectionManager;
        dialog.SimulationManager = simulationManager;
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

    public void OnNewSimulationPressed()
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

    public void OnDeleteSimulationPressed()
    {
        var dialog = uiManager.ShowConfirmationDialog(@$"Are you sure you want to delete ""{simulationManager.CurrentSimulation.name}""?");
        dialog.okayEvent.AddListener(() =>
        {
            var spinner = uiManager.ShowSpinner();
            simulationManager.Delete(simulationManager.CurrentSimulation.id,
                simulation =>
                {
                    spinner.Hide();
                });
        });    
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