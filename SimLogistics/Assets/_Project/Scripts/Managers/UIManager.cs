using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    // Events
    [SerializeField] private UnityEvent bootstrapEvent;
    [SerializeField] private UnityEvent bootstrapCompleteEvent;

    // Interaction
    [SerializeField] private BoolVariable isWorldInteractable;

    // Dialogs
    [SerializeField] private UIElement backdrop;
    [SerializeField] private Transform dialogHost;
    [SerializeField] private ConfirmationDialog confirmationDialogPrefab;
    [SerializeField] private ErrorDialog errorDialogPrefab;
    [SerializeField] private InfoDialog infoDialogPrefab;
    [SerializeField] private HelpDialog helpDialogPrefab;

    private readonly Queue<Dialog> _dialogQueue = new Queue<Dialog>();
    private Dialog _activeDialog;
    public bool IsBootstrappedTriggered { get; private set; }

    private void Start()
    {
        DisableWorldInteraction();
    }

    public void SignalBootstrapComplete()
    {
        bootstrapCompleteEvent.Invoke();
    }
    
    // Connection state
    // ----------------

    public void OnConnected()
    {
        print($"UIM: OnConnected: IsBootstrapped: {IsBootstrappedTriggered}");
        
        if (IsBootstrappedTriggered) return;
        bootstrapEvent.Invoke();
        IsBootstrappedTriggered = true;
    }

    public void OnDisconnected()
    {
        DisableWorldInteraction();
    }

    public void OnConnectionError(string error)
    {
        DisableWorldInteraction();
        ShowErrorDialog($"Connection Error{Environment.NewLine}{error}");
    }

    // Interaction state
    // -----------------

    public void EnableWorldInteraction()
    {
        isWorldInteractable.Value = true;
    }

    public void DisableWorldInteraction()
    {
        isWorldInteractable.Value = false;
    }

    // Dialogs
    // -------

    public T ShowDialogPrefab<T>(Dialog prefab) where T : Dialog
    {
        var dialog = Instantiate(prefab, dialogHost, false) as T;
        Debug.Assert(dialog != null);
        ShowDialog(dialog);
        return dialog;
    }
    
    public void ShowDialog(Dialog dialog)
    {
        Debug.Assert(dialog != null);
        
        _dialogQueue.Enqueue(dialog);
        ShowQueuedDialog();
    }

    private void ShowQueuedDialog()
    {
        if (_dialogQueue.Count == 0 || _activeDialog) return;

        DisableWorldInteraction();

        backdrop.gameObject.SetActive(true);
        backdrop.SetVisible(true, UIElement.Effect.Fade);
        
        _activeDialog = _dialogQueue.Peek();
        print($"showing {_dialogQueue.Count}: " + _activeDialog);
        _activeDialog.UIManager = this;
        _activeDialog.SetVisible(true, UIElement.Effect.Animate);
    }

    public void HideDialog(Dialog dialog)
    {
        Debug.Assert(dialog == _activeDialog);

        dialog
            .SetVisible(false, UIElement.Effect.Animate)
            .setOnComplete(() =>
                {
                    _dialogQueue.Dequeue();
                    _activeDialog = null;
                    if (_dialogQueue.Count == 0)
                    {
                        backdrop.SetVisible(false, UIElement.Effect.Fade);
                        EnableWorldInteraction();
                    }
                    else
                    {
                        ShowQueuedDialog();
                    }
                }
            )
            .destroyOnComplete = true;
    }

    public void ShowConfirmationDialog(string message)
    {
        var confirmationDialog = Instantiate(confirmationDialogPrefab, dialogHost, false);
        confirmationDialog.SetText(message);
        ShowDialog(confirmationDialog);
    }

    public void ShowErrorDialog(string message)
    {
        ErrorDialog errorDialog;

        foreach (var dialog in _dialogQueue)
        {
            errorDialog = dialog as ErrorDialog;
            if (errorDialog == null) continue;

            if (errorDialog.GetText() != message) continue;
            
            return;
        }

        errorDialog = ShowDialogPrefab<ErrorDialog>(errorDialogPrefab);
        errorDialog.SetText(message);
    }

    // Info and Help
    // -------------

    public void OnInfo()
    {
        var infoDialog = ShowDialogPrefab<InfoDialog>(infoDialogPrefab);
        infoDialog.SetText("TODO: Change this to Info dialog");
    }

    public void OnHelp()
    {
        var helpDialog = ShowDialogPrefab<HelpDialog>(helpDialogPrefab);
        helpDialog.SetText("TODO: Change this to Help dialog");
    }
}