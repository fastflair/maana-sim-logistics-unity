using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Actions : MonoBehaviour
{
    // Managers
    [SerializeField] private UIManager uiManager;
    [SerializeField] private CityManager cityManager;
    [SerializeField] private ProducerManager producerManager;
    [SerializeField] private HubManager hubManager;
    [SerializeField] private VehicleManager vehicleManager;
    [SerializeField] private SimulationManager simulationManager;

    // Prefabs
    [SerializeField] private ToggleItem toggleItemPrefab;
    [SerializeField] private TransitOrderItem transitOrderItemPrefab;

    [SerializeField] private TransferOrderItem transferOrderItemPrefab;

    // List hosts
    [SerializeField] private Transform transitActionsListHost;
    [SerializeField] private Transform repairActionsListHost;
    [SerializeField] private Transform transferActionsListHost;

    [SerializeField] private Transform ordersListHost;

    // Buttons
    [SerializeField] private Button clearAllButton;

    [SerializeField] private Button clearUntoggledButton;

    // Tab controls (to self activate)
    [SerializeField] private TabGroup tabGroup;
    [SerializeField] private Tab issuedTab;

    private readonly List<ToggleItem> _toggleItems = new List<ToggleItem>();

    private void Start()
    {
        UpdateButtons();
    }

    public void OnNewAction(SimulationManager.ActionInfo info)
    {
        var host = GetActionHost(info.Type);
        var item = Instantiate(toggleItemPrefab, host, false);
        item.data = info;
        item.Label = info.DisplayText;
        item.onValueChanged.AddListener(UpdateButtons);
        _toggleItems.Add(item);
        UpdateButtons();
    }

    public void OnActionsReset()
    {
        ClearPendingList();
    }

    public void OnUpdateIssuedOrders()
    {
        ClearIssuedList();

        var transferOrders = simulationManager.CurrentState.transfers;
        transferOrders.Sort(SimulationManager.TransferComparer);
        foreach (var transferOrder in transferOrders)
        {
            var transferOrderItem = Instantiate(transferOrderItemPrefab, ordersListHost, false);
            transferOrderItem.CityManager = cityManager;
            transferOrderItem.ProducerManager = producerManager;
            transferOrderItem.HubManager = hubManager;
            transferOrderItem.VehicleManager = vehicleManager;
            transferOrderItem.Populate(transferOrder);
        }

        var transitOrders = simulationManager.CurrentState.vehicles.Select(x => x.transitOrder);
        var qTransitOrders = transitOrders.ToList();
        qTransitOrders.Sort(SimulationManager.TransitComparer);

        foreach (var transitOrder in qTransitOrders)
        {
            var transitOrderItem = Instantiate(transitOrderItemPrefab, ordersListHost, false);
            transitOrderItem.VehicleManager = vehicleManager;
            transitOrderItem.Populate(transitOrder);
        }

        tabGroup.OnTabClick(issuedTab);
    }

    public void UpdateButtons()
    {
        clearAllButton.interactable = _toggleItems.Count > 0;
        clearUntoggledButton.interactable = GetUntoggledItems().Any();
    }

    public void OnClearUntoggled()
    {
        var items = GetUntoggledItems();
        var enumerable = items.ToList();
        var dialog = uiManager.ShowConfirmationDialog(
            $"Are you sure you want remove {enumerable.Count} {(enumerable.Count > 1 ? "actions" : "action")}?");
        dialog.onOkay.AddListener(() =>
        {
            foreach (var item in enumerable)
            {
                var info = (SimulationManager.ActionInfo) item.data;
                switch (info.Type)
                {
                    case SimulationManager.ActionType.Transit:
                        simulationManager.RemoveTransitAction(info.ID);
                        break;
                    case SimulationManager.ActionType.Repair:
                        simulationManager.RemoveRepairAction(info.ID);
                        break;
                    case SimulationManager.ActionType.Transfer:
                        simulationManager.RemoveTransferAction(info.ID);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _toggleItems.Remove(item);
                Destroy(item.gameObject);
            }

            UpdateButtons();
        });
    }

    public void OnClearAll()
    {
        var dialog = uiManager.ShowConfirmationDialog("Are you sure you want remove all pending actions?");
        dialog.onOkay.AddListener(() =>
        {
            simulationManager.ResetActions();
            ClearPendingList();
        });
    }

    // --- Internal

    private IEnumerable<ToggleItem> GetUntoggledItems()
    {
        return _toggleItems.Where(x => !x.IsOn);
    }

    private Transform GetActionHost(SimulationManager.ActionType type)
    {
        return type switch
        {
            SimulationManager.ActionType.Transit => transitActionsListHost,
            SimulationManager.ActionType.Repair => repairActionsListHost,
            SimulationManager.ActionType.Transfer => transferActionsListHost,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private void ClearPendingList()
    {
        _toggleItems.ForEach(x => Destroy(x.gameObject));
        _toggleItems.Clear();
        UpdateButtons();
    }

    private void ClearIssuedList()
    {
        foreach (Transform child in ordersListHost)
            Destroy(child.gameObject);
    }
}