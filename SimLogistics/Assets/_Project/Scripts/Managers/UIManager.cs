using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Events
    [SerializeField] private UnityEvent onBootstrap;

    // State
    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private SimulationManager simulationManager;
    
    // Interaction
    [SerializeField] private BoolVariable isWorldInteractable;

    // Title
    [SerializeField] private GameObject title;

    // HUD
    [SerializeField] private GameObject hud;
    [SerializeField] private float hudFadeDuration;
    [SerializeField] private float hudFadeDelay;

    // HUD: Bars and Buttons
    [SerializeField] private SystemBar systemBar;
    [SerializeField] private SimulationBar simulationBar;

    // Dialogs
    [SerializeField] private GameObject backdrop;
    [SerializeField] private Transform dialogHost;
    [SerializeField] private ConfirmationDialog confirmationDialog;
    [SerializeField] private MessageDialog errorDialog;
    [SerializeField] private MessageDialog infoDialog;
    [SerializeField] private MessageDialog helpDialog;
    
    private void Start()
    {
        // Initial conditions
        DisableWorldInteraction();
        // HideHUD();
        systemBar.ConnectionNotReady();
        ShowSystemBar();
        
        ShowTitle();

        onBootstrap.Invoke();
    }

    // Connection state
    // ----------------

    public void OnConnected()
    {
        systemBar.ConnectionReady();
    }

    public void OnDisconnected()
    {
        systemBar.ConnectionNotReady();
    }
    
    public void OnConnectionError(string error)
    {
        print("Connection error: " + error);
        ShowErrorDialog("Connection error: " + error);
        
        systemBar.ConnectionNotReady();
        ShowSystemBar();
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

    // Title
    // -----
    
    public void ShowTitle()
    {
        title.SetActive(true);
    }

    public void HideTitle()
    {
        title.SetActive(false);
    }

    // HUD
    // ---
    
    public void FadeHUDIn()
    {
        hud.GetComponent<CanvasGroup>().alpha = 0f;
        ShowHUD();
        LeanTween.alphaCanvas(hud.GetComponent<CanvasGroup>(), 1f, hudFadeDuration).setDelay(hudFadeDelay);
    }

    public void FadeHUDOut()
    {
        hud.GetComponent<CanvasGroup>().alpha = 1f;
        LeanTween.alphaCanvas(hud.GetComponent<CanvasGroup>(), 0f, hudFadeDuration).setDelay(hudFadeDelay)
            .setOnComplete(HideHUD);
    }

    public void ShowHUD()
    {
        hud.SetActive(true);
    }

    public void HideHUD()
    {
        hud.SetActive(false);
    }

    // HUD: Button Bars
    // ----------------
    
    public void ShowSystemBar()
    {
        HideSimulationBar();
        systemBar.Show();
    }

    public void HideSystemBar()
    {
        systemBar.Hide();
    }

    public void ShowSimulationBar()
    {
        HideSystemBar();
        simulationBar.Show();
    }

    public void HideSimulationBar()
    {
        simulationBar.Hide();
    }
    
    // Dialogs
    // -------
    
    public T CreateAndShowDialog<T>(Dialog prefab) where T : class
    {
        var dialog = Instantiate(prefab, dialogHost, false);
        dialog.UIManager = this;
        ShowDialog(dialog);
        return dialog as T;
    }

    public void ShowDialog(Dialog dialog)
    {
        DisableWorldInteraction();
        
        backdrop.GetComponent<CanvasGroup>().alpha = 0f;
        backdrop.SetActive(true);

        LeanTween.alphaCanvas(backdrop.GetComponent<CanvasGroup>(), 1f, 1f);
        
        var slide = dialog.GetComponent<SlideUI>();
        slide.SetVisible(true);
    }

    public void HideDialog(Dialog dialog)
    {
        var slide = dialog.GetComponent<SlideUI>();
        slide.SetVisible(false);
        
        LeanTween.alphaCanvas(backdrop.GetComponent<CanvasGroup>(), 0f, 1f).setOnComplete(() =>
        {
            Destroy(dialog);
            backdrop.SetActive(false);
            EnableWorldInteraction();
        });
    }
    
    public void ShowConfirmationDialog(string message)
    {
        CreateAndShowDialog<ConfirmationDialog>(confirmationDialog);
    }
    
    public void ShowErrorDialog(string message)
    {
        var dialog = CreateAndShowDialog<MessageDialog>(errorDialog);
        dialog.SetText(message);
    }

    // Info and Help
    // -------------
    
    public void OnInfo()
    {
        var dialog = CreateAndShowDialog<MessageDialog>(infoDialog);
        dialog.SetText("TODO: Change this to Info dialog");
    }

    public void OnHelp()
    {
        var dialog = CreateAndShowDialog<MessageDialog>(helpDialog);
        dialog.SetText("TODO: Change this to Help dialog");
    }

}