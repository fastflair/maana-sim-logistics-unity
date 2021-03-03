using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    // Events
    [SerializeField] private UnityEvent onTitleSequenceStart;
    [SerializeField] private UnityEvent onTitleSequenceCompleted;

    // Interaction
    [SerializeField] private BoolVariable isWorldInteractable;

    // Dialogs
    [SerializeField] private UIElement backdrop;
    [SerializeField] private Transform dialogHost;
    [SerializeField] private ConfirmationDialog confirmationDialogPrefab;
    [SerializeField] private ErrorDialog errorDialogPrefab;
    [SerializeField] private InfoDialog infoDialogPrefab;
    [SerializeField] private HelpDialog helpDialogPrefab;
    [SerializeField] private Spinner spinnerPrefab;

    private readonly Queue<Dialog> _dialogQueue = new Queue<Dialog>();

    public bool IsTitleSequenceTriggered { get; private set; }

    private void Start()
    {
        DisableWorldInteraction();
    }

    // Title Sequence
    // --------------
    
    public void StartTitleSequence()
    {
        if (IsTitleSequenceTriggered) return;
        IsTitleSequenceTriggered = true;
        onTitleSequenceStart.Invoke();
    }

    public void EndTitleSequence()
    {
        onTitleSequenceCompleted.Invoke();
    }

    // Connection state
    // ----------------
    
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
        InternalShowDialog(dialog);
    }

    private void InternalShowDialog(Dialog dialog)
    {
        if (_dialogQueue.Count == 1)
        {
            DisableWorldInteraction();

            backdrop.gameObject.SetActive(true);
            backdrop.SetVisible(true, UIElement.Effect.Fade);
        }
        
        print($"showing {_dialogQueue.Count}: " + dialog);
        dialog.UIManager = this;
        dialog.SetVisible(true, UIElement.Effect.Animate);
    }

    public void HideDialog(Dialog dialog)
    {
        Debug.Assert(dialog != null);

        dialog
            .SetVisible(false, UIElement.Effect.Animate)
            .setOnComplete(() =>
                {
                    _dialogQueue.Dequeue();
                    if (_dialogQueue.Count != 0) return;
                    backdrop.SetVisible(false, UIElement.Effect.Fade).setOnComplete(() =>
                        backdrop.gameObject.SetActive(false));

                    EnableWorldInteraction();
                }
            )
            .destroyOnComplete = true;
    }

    public ConfirmationDialog ShowConfirmationDialog(string message)
    {
        var confirmationDialog = Instantiate(confirmationDialogPrefab, dialogHost, false);
        confirmationDialog.SetText(message);
        ShowDialog(confirmationDialog);
        return confirmationDialog;
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
    
    // Spinner
    // -------

    public Spinner ShowSpinner()
    {
        return ShowDialogPrefab<Spinner>(spinnerPrefab);
    }
}