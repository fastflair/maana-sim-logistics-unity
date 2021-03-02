using UnityEngine;
using UnityEngine.UI;

public class SystemBar : MonoBehaviour
{
    // Managers
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ConnectionManager connectionManager;

    // Dialogs
    [SerializeField] private ConnectionsDialog connectionsDialog;
    [SerializeField] private NewSimulationDialog newSimulationDialog;
    [SerializeField] private LoadSimulationDialog loadSimulationDialog;

    // Buttons
    [SerializeField] private Button newSimulationButton;
    [SerializeField] private Button loadSimulationButton;
    [SerializeField] private Button deleteSimulationButton;
    [SerializeField] private Button simulationButton;

    // State management
    // ----------------

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ConnectionReady()
    {
        EnableNewSimulationButton();
        EnableLoadSimulationButton();
    }

    public void ConnectionNotReady()
    {
        DisableNewSimulationButton();
        DisableLoadSimulationButton();
        DisableDeleteSimulationButton();
        SimulationNotReady();
    }

    public void SimulationReady()
    {
        EnableSimulationButton();
    }

    public void SimulationNotReady()
    {
        DisableSimulationButton();
    }

    // Connections
    // -----------

    public void OnConnections()
    {
        var dialog = uiManager.CreateAndShowDialog<ConnectionsDialog>(connectionsDialog);
        dialog.ConnectionManager = connectionManager;
    }

    // Load
    // ----

    public void OnLoadSimulation()
    {
        uiManager.CreateAndShowDialog<LoadSimulationDialog>(loadSimulationDialog);
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
        uiManager.CreateAndShowDialog<NewSimulationDialog>(newSimulationDialog);
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