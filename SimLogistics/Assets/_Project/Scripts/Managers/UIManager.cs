using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    // Events
    [SerializeField] private UnityEvent onLoading;
    [SerializeField] private UnityEvent onReady;
    [SerializeField] private UnityEvent onNotReady;

    // Interaction
    [SerializeField] private BoolVariable isWorldInteractable;

    // Dialogs
    [SerializeField] private DepthOfFieldController dofController;
    [SerializeField] private UIElement backdrop;
    [SerializeField] private Transform dialogHost;
    [SerializeField] private ConfirmationDialog confirmationDialogPrefab;
    [SerializeField] private ErrorDialog errorDialogPrefab;
    [SerializeField] private ErrorDialog graphQlDialogPrefab;
    [SerializeField] private InfoDialog infoDialogPrefab;
    [SerializeField] private HelpDialog helpDialogPrefab;

    private readonly Queue<Dialog> _dialogQueue = new Queue<Dialog>();

    // Cards
    [SerializeField] private Transform cardShelf;
    private Card _currentCard;
    private string _currentCardId;

    // Spinner
    [SerializeField] private Spinner spinnerPrefab;
    private Spinner _spinner;

    private void Start()
    {
        _spinner = Instantiate(spinnerPrefab, dialogHost, false);

        DisableWorldInteraction();
    }

    // Connection state
    // ----------------

    public void OnConnectionError(string error)
    {
        DisableWorldInteraction();
        ShowErrorDialog($"Connection Error{Environment.NewLine}{error}");
    }

    // Interaction state
    // -----------------

    public void Loading()
    {
        NotReady();
        onLoading.Invoke();
    }

    public void Ready()
    {
        onReady.Invoke();
        EnableWorldInteraction();
    }

    public void NotReady()
    {
        DisableWorldInteraction();
        onNotReady.Invoke();
    }

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

            dofController.BlurBackground();
        }

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

                    dofController.UnblurBackground();
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

    public void ShowGraphQlDialog(string message)
    {
        var dialog = ShowDialogPrefab<ErrorDialog>(graphQlDialogPrefab);
        dialog.SetText(message);
    }

    // Info and Help
    // -------------

    public void OnInfo()
    {
        ShowInfoDialog("TODO: Change this to Info dialog");
    }

    public void ShowInfoDialog(string message)
    {
        var infoDialog = ShowDialogPrefab<InfoDialog>(infoDialogPrefab);
        infoDialog.SetText(message);
    }

    public void OnHelp()
    {
        ShowHelpDialog("TODO: Change this to Help dialog");
    }

    public void ShowHelpDialog(string message)
    {
        var helpDialog = ShowDialogPrefab<HelpDialog>(helpDialogPrefab);
        helpDialog.SetText(message);
    }

    // Spinner
    // -------

    public void ShowSpinner()
    {
        _spinner.transform.SetAsLastSibling();
        _spinner.SetVisible(true, UIElement.Effect.None);
    }

    public void HideSpinner()
    {
        _spinner.SetVisible(false, UIElement.Effect.None);
    }

    // Cards
    // -----
    public Card SpawnCard(Card cardPrefab, string id)
    {
        return _currentCardId == id ? null : Instantiate(cardPrefab, cardShelf, false);
    }
    
    private DebounceDispatcher _debouncer = new DebounceDispatcher(1);

    public void ShowCard(Card card, string id)
    {
        if (_currentCardId == id) return;

        if (_currentCard != null)
        {
            _currentCard.SetVisible(false, UIElement.Effect.Animate)
                .destroyOnComplete = true;
        }

        _currentCard = card;
        _currentCardId = id;
        _currentCard.SetVisible(true, UIElement.Effect.Animate);
    }
}