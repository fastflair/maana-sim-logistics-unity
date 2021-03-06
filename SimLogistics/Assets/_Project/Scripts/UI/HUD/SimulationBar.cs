using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SimulationBar : UIElement
{
    private const string MoveHelpSelection = "At least one vehicle and one structure must be selected.";

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

    public void OnMovePressed()
    {
        var vehicleStructurePair = GetSelectedVehicleStructurePair();
        if (vehicleStructurePair == null)
        {
            uiManager.ShowHelpDialog(MoveHelpSelection);
            return;
        }

        simulationManager.MoveVehicleTo(
            vehicleStructurePair.Vehicle.id,
            vehicleStructurePair.Structure.x,
            vehicleStructurePair.Structure.y,
            status => { uiManager.ShowInfoDialog($"Transit order status: {status}"); });
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
        simulationManager.Step(state => { });
    }

    public void OnThinkPressed()
    {
        simulationManager.Think();
    }

    public void EnableThinkButton()
    {
        thinkButton.interactable = true;
    }

    public void DisableThinkButton()
    {
        thinkButton.interactable = false;
    }

    // Button actions
    // --------------

    private class VehicleStructurePair
    {
        public QEntity Structure;
        public QEntity Vehicle;
    }
}