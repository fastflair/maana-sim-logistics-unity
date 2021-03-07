using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class Actions : UIElement
{
    [SerializeField] private CheckItem checkItemPrefab;
    [SerializeField] private Transform transitActionsListHost;
    [SerializeField] private Transform repairActionsListHost;
    [SerializeField] private Transform transferActionsListHost;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SimulationManager simulationManager;
    [SerializeField] private Button clearAllButton;
    [SerializeField] private Button clearUncheckedButton;

    private readonly List<CheckItem> _checkItems = new List<CheckItem>();

    private void Start()
    {
        UpdateButtons();
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

    public void OnReady()
    {
        if (!simulationManager.IsCurrentDefault) ShowAnimate();
    }
    
    public void OnNewAction(SimulationManager.ActionInfo info)
    {
        var host = GetActionHost(info.Type);
        var item = Instantiate(checkItemPrefab, host.transform, false);
        item.data = info;
        item.Label = info.DisplayText;
        item.onValueChanged.AddListener(UpdateButtons);
        _checkItems.Add(item);
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        clearAllButton.interactable = _checkItems.Count > 0;
        clearUncheckedButton.interactable = GetUncheckedItems().Any();
    }
    
    public void OnClearUnchecked()
    {
        var items = GetUncheckedItems();
        var enumerable = items.ToList();
        var dialog = uiManager.ShowConfirmationDialog(
            $"Are you sure you want remove {enumerable.Count} {(enumerable.Count > 1 ? "actions" : "action")}?");
        dialog.okayEvent.AddListener(() =>
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

                _checkItems.Remove(item);
                Destroy(item.gameObject);
            }
            
            UpdateButtons();
        });
    }

    public void OnClearAll()
    {
        var dialog = uiManager.ShowConfirmationDialog("Are you sure you want remove all pending actions?");
        dialog.okayEvent.AddListener(() =>
        {
            simulationManager.ResetActions(); 
            ClearList();
        });
    }

    // --- Internal
    
    private IEnumerable<CheckItem> GetUncheckedItems()
    {
        return _checkItems.Where(x => !x.IsChecked);
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
        _checkItems.ForEach(x => Destroy(x.gameObject));
        _checkItems.Clear();
        UpdateButtons();
    }
}