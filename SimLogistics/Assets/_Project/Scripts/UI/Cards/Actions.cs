using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Actions : UIElement
{
    [SerializeField] private ButtonItem buttonItemPrefab;
    [SerializeField] private Transform transitActionsListHost;
    [SerializeField] private Transform repairActionsListHost;
    [SerializeField] private Transform transferActionsListHost;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SimulationManager simulationManager;
    [SerializeField] private Button removeAllButton;
    [SerializeField] private Button removeAllSelected;

    private void Start()
    {
        Show();
    }

    public void Show()
    {
        SetVisible(true);
    }

    public void ShowAnimate()
    {
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

    public void OnActionsUpdate()
    {
        Populate();
    }

    public void OnRemoveSelected()
    {
    }

    public void OnRemoveAll()
    {
        var dialog = uiManager.ShowConfirmationDialog("Are you sure you want remove all pending actions?");
        dialog.okayEvent.AddListener(() => { simulationManager.ResetActions(); });
    }

    private static string TransitActionFormatter(QTransitAction action)
    {
        var vehicle = SimulationManager.FormatEntityId(action.vehicle);
        return $"{vehicle} → ({action.destX}, {action.destY})";
    }

    private static string RepairActionFormatter(QRepairAction action)
    {
        var vehicle = SimulationManager.FormatEntityId(action.vehicle);
        var hub = SimulationManager.FormatEntityId(action.hub);
        return $"{vehicle} → {hub})";
    }

    private static string TransferActionFormatter(QTransferAction action)
    {
        var vehicle = SimulationManager.FormatEntityId(action.vehicle);
        var counterparty = SimulationManager.FormatEntityId(action.counterparty);
        var resource = SimulationManager.FormatEntityId(action.resourceType.id);
        var dir = action.transferType.id == "Withdrawal" ? "←" : "→";
        return $"{vehicle} {dir} {counterparty}: {resource} x {action.quantity})";
    }

    private void Populate()
    {
        ClearList(transitActionsListHost);
        PopulateList(
            transitActionsListHost,
            simulationManager.Actions.transitActions,
            TransitActionFormatter);

        ClearList(repairActionsListHost);
        PopulateList(
            repairActionsListHost,
            simulationManager.Actions.repairActions,
            RepairActionFormatter);

        ClearList(transferActionsListHost);
        PopulateList(
            transferActionsListHost,
            simulationManager.Actions.transferActions,
            TransferActionFormatter);

        removeAllButton.interactable =
            removeAllButton.interactable = AnyActionsInList();
    }

    private void PopulateList<T>(
        Component host,
        IEnumerable<T> list,
        Formatter<T> formatter)
    {
        foreach (var action in list)
        {
            var buttonItem = Instantiate(buttonItemPrefab, host.transform, false);
            buttonItem.Label = formatter(action);
        }
    }

    private bool AnyActionsInList()
    {
        return simulationManager.Actions.transitActions.Count > 0 ||
               simulationManager.Actions.repairActions.Count > 0 ||
               simulationManager.Actions.transferActions.Count > 0;
    }

    private static void ClearList(IEnumerable host)
    {
        foreach (Transform buttonItem in host) Destroy(buttonItem.gameObject);
    }

    private delegate string Formatter<in T>(T action);
}