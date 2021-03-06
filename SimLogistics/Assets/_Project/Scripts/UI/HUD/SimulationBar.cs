using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SimulationBar : UIElement
{
    private const string MoveHelpSelection = "At least one vehicle and one structure must be selected.";
    private const string RepairHelpSelection = "One vehicle must be selected for repair.";

    // Managers
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SimulationManager simulationManager;
    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private SelectionManager selectionManager;

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
        if (!simulationManager.IsCurrentDefault) ShowAnimate();
    }

    private void UpdateButtons()
    {
        if (simulationManager.AgentEndpoint != null)
            EnableThinkButton();
        else
            DisableThinkButton();
    }

    private class VehicleStructurePair
    {
        public QEntity Structure;
        public QEntity Vehicle;
    }

    private VehicleStructurePair GetSelectedVehicleStructurePair()
    {
        var selectableObjects = selectionManager.SelectedObjects.ToArray();
        if (selectableObjects.Length != 2) return null;

        var entity1 = selectableObjects[0].GetComponent<Entity>();
        var entity2 = selectableObjects[1].GetComponent<Entity>();

        QEntity vehicle = null;
        QEntity structure = null;

        if (entity1.QEntity is QVehicle)
        {
            vehicle = entity1.QEntity;
            if (!(entity2.QEntity is QVehicle)) structure = entity2.QEntity;
        }
        else if (entity2.QEntity is QVehicle)
        {
            vehicle = entity2.QEntity;
            structure = entity1.QEntity;
        }

        if (vehicle == null || structure == null) return null;

        return new VehicleStructurePair
        {
            Vehicle = vehicle,
            Structure = structure
        };
    }

    private QVehicle GetSelectedVehicle()
    {
        var selectableObjects = selectionManager.SelectedObjects.ToArray();
        if (selectableObjects.Length != 1) return null;

        return selectableObjects[0].GetComponent<Entity>().QEntity as QVehicle;
    }
    
    // Buttons
    // -------
    
    public void OnMovePressed()
    {
        var vehicleStructurePair = GetSelectedVehicleStructurePair();
        if (vehicleStructurePair == null)
        {
            uiManager.ShowHelpDialog(MoveHelpSelection);
            return;
        }

        var vehicleId = vehicleStructurePair.Vehicle.id;

        simulationManager.AddTransitAction(
            vehicleId,
            vehicleStructurePair.Structure.x,
            vehicleStructurePair.Structure.y
        );
        
        selectionManager.DeselectAll();
    }

    public void OnRepairPressed()
    {
        var vehicle = GetSelectedVehicle();
        if (vehicle == null)
        {
            uiManager.ShowHelpDialog(RepairHelpSelection);
            return;
        }

        simulationManager.AddRepairAction(vehicle.id, vehicle.hub);
        
        selectionManager.DeselectAll();
    }

    public void OnTransferPressed()
    {
        print($"[{name}] OnTransferPressed");
        // Actions.transferActions.Add(
        //     new QTransferAction()
        //     {
        //         id = $"{CurrentSimulation.id}:{vehicle}",
        //         vehicle = vehicle,
        //         counterparty = counterParty,
        //         resourceType = res
        //     });
    }

    public void OnSimulatePressed()
    {
        simulationManager.Simulate(state => { });
    }

    public void OnThinkPressed()
    {
        simulationManager.Think(
            simulationManager.AgentEndpoint,
            simulationManager.CurrentState,
            actions =>
            {
                // TODO: add to actions queue
            });
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