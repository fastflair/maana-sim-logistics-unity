using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Actions : MonoBehaviour
{
    [SerializeField] private ToggleItem toggleItemPrefab;
    [SerializeField] private Transform transitActionsListHost;
    [SerializeField] private Transform repairActionsListHost;
    [SerializeField] private Transform transferActionsListHost;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SimulationManager simulationManager;
    [SerializeField] private Button clearAllButton;
    [SerializeField] private Button clearUntoggledButton;

    private readonly List<ToggleItem> _toggleItems = new List<ToggleItem>();

    private void Start()
    {
        UpdateButtons();
    }
    
    public void OnNewAction(SimulationManager.ActionInfo info)
    {
        var host = GetActionHost(info.Type);
        var item = Instantiate(toggleItemPrefab, host.transform, false);
        item.data = info;
        item.Label = info.DisplayText;
        item.onValueChanged.AddListener(UpdateButtons);
        _toggleItems.Add(item);
        UpdateButtons();
    }

    public void OnActionsReset()
    {
        ClearList();
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
            ClearList();
        });
    }

    // --- Internal
    
    private IEnumerable<ToggleItem> GetUntoggledItems()
    {
        _toggleItems.ForEach(x =>
        {
            print($"ToggleItem: {x.Label} = {x.IsOn}");
        });

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

    private void ClearList()
    {
        _toggleItems.ForEach(x => Destroy(x.gameObject));
        _toggleItems.Clear();
        UpdateButtons();
    }
}