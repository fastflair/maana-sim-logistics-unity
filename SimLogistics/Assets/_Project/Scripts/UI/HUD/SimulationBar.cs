using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SimulationBar : UIElement
{
    private const string MoveHelpSelection = "One vehicle and one structure must be selected for transit.";
    private const string RepairHelpSelection = "One vehicle must be selected for repair.";
    private const string TransferHelpSelection = "One vehicle and either a producer or city must be selected for resource transfer.";

    // Managers
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SimulationManager simulationManager;
    [SerializeField] private SelectionManager selectionManager;

    // Dialogs
    [SerializeField] private TransferDialog transferDialog;

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

    private VehicleStructurePair GetSelectedVehicleStructurePair()
    {
        var selectableObjects = selectionManager.SelectedObjects.ToArray();
        if (selectableObjects.Length != 2) return null;

        var entity1 = selectableObjects[0].GetComponent<Entity>();
        var entity2 = selectableObjects[1].GetComponent<Entity>();

        QVehicle vehicle = null;
        QEntity structure = null;

        if (entity1.QEntity is QVehicle entity)
        {
            vehicle = entity;
            if (!(entity2.QEntity is QVehicle)) structure = entity2.QEntity;
        }
        else if (entity2.QEntity is QVehicle qEntity)
        {
            vehicle = qEntity;
            structure = entity1.QEntity;
        }

        if (vehicle == null || structure == null) return null;

        return new VehicleStructurePair
        {
            Vehicle = vehicle,
            Structure = structure,
            City = structure as QCity,
            Hub = structure as QHub,
            Producer = structure as QProducer
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
        var pair = GetSelectedVehicleStructurePair();
        if (pair == null)
        {
            uiManager.ShowHelpDialog(MoveHelpSelection);
            return;
        }

        var vehicleId = pair.Vehicle.id;

        simulationManager.AddTransitAction(
            vehicleId,
            pair.Structure.x,
            pair.Structure.y
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
        var pair = GetSelectedVehicleStructurePair();
        if (pair == null || pair.City == null && pair.Producer == null)
        {
            uiManager.ShowHelpDialog(TransferHelpSelection);
            return;
        }

        var dialog = uiManager.ShowDialogPrefab<TransferDialog>(transferDialog);
        dialog.SimulationManager = simulationManager;
        dialog.Vehicle = pair.Vehicle;
        dialog.Producer = pair.Producer;
        dialog.City = pair.City;

        dialog.onAddTransfer.AddListener(() =>
        {
            simulationManager.AddTransferAction(
                pair.Vehicle.id,
                pair.Structure.id,
                new QResourceTypeEnum {id = dialog.ResourceType},
                dialog.Quantity,
                new QResourceTransferTypeEnum {id = dialog.TransferType});

            selectionManager.DeselectAll();
        });
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

    private class VehicleStructurePair
    {
        public QProducer Producer;
        public QCity City;
        public QHub Hub;
        public QEntity Structure;
        public QVehicle Vehicle;
    }
}