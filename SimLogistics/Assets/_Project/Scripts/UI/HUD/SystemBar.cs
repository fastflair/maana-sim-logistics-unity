using UnityEngine;
using UnityEngine.UI;

public class SystemBar : UIElement
{
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

    // Scenario editor
    [SerializeField] private ScenarioEditor scenarioEditor;

    public override void Show()
    {
        UpdateButtons();
        base.Show();
    }

    public override void ShowAnimate()
    {
        UpdateButtons();
        base.ShowAnimate();
    }
    
    public void OnReady()
    {
        if (simulationManager.IsCurrentDefault) ShowAnimate();
    }

    private void UpdateButtons()
    {
        if (simulationManager.IsCurrentDefault)
        {
            DisableDeleteSimulationButton();
            DisableSimulationButton();
        }
        else
        {
            EnableDeleteSimulationButton();
            EnableSimulationButton();
        }
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
        dialog.ScenarioEditor = scenarioEditor;
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
        var dialog =
            uiManager.ShowConfirmationDialog(
                @$"Are you sure you want to delete ""{simulationManager.CurrentSimulation.name}""?");
        dialog.onOkay.AddListener(() =>
        {
            simulationManager.Delete(simulationManager.CurrentSimulation.id,
                sim => { });
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