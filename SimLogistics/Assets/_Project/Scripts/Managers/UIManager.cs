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
    [SerializeField] private GameObject systemBar;
    [SerializeField] private GameObject simulationBar;
    private Button _newSimulationButton;
    private Button _loadSimulationButton;
    private Button _deleteSimulationButton;
    private Button _simulationButton;

    // Dialogs
    [SerializeField] private GameObject backdrop;
    [SerializeField] private ConnectionsDialog connectionsDialog;

    private void Awake()
    {
        InitializeSystemButtons();
    }

    private void Start()
    {
        
        // Initial conditions
        DisableWorldInteraction();
        // HideHUD();
        SimulationNotReady();
        ShowSystemBar();
        
        ShowTitle();

        onBootstrap.Invoke();
    }

    private void InitializeSystemButtons()
    {
        _newSimulationButton = systemBar.transform.Find("New").GetComponent<Button>();
        _loadSimulationButton = systemBar.transform.Find("Load").GetComponent<Button>();
        _deleteSimulationButton = systemBar.transform.Find("Delete").GetComponent<Button>();
        _simulationButton = systemBar.transform.Find("Simulation").GetComponent<Button>();
    }
    
    public void SimulationReady()
    {
        EnableNewSimulationButton();
        EnableLoadSimulationButton();
        EnableDeleteSimulationButton();
        EnableSimulationButton();
    }

    public void SimulationNotReady()
    {
        DisableNewSimulationButton();
        DisableLoadSimulationButton();
        DisableDeleteSimulationButton();
        DisableSimulationButton();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void EnableWorldInteraction()
    {
        isWorldInteractable.Value = true;
    }

    public void DisableWorldInteraction()
    {
        isWorldInteractable.Value = false;
    }

    // Title
    public void ShowTitle()
    {
        title.SetActive(true);
    }

    public void HideTitle()
    {
        title.SetActive(false);
    }

    // HUD
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
    public void ShowSystemBar()
    {
        HideSimulationBar();
        systemBar.SetActive(true);
    }

    public void HideSystemBar()
    {
        systemBar.SetActive(false);
    }

    public void ShowSimulationBar()
    {
        HideSystemBar();
        simulationBar.SetActive(true);
    }

    public void HideSimulationBar()
    {
        simulationBar.SetActive(false);
    }

    public void EnableNewSimulationButton()
    {
        _newSimulationButton.interactable = true;
    }

    public void DisableNewSimulationButton()
    {
        _newSimulationButton.interactable = false;
    }

    public void EnableLoadSimulationButton()
    {
        _loadSimulationButton.interactable = true;
    }

    public void DisableLoadSimulationButton()
    {
        _loadSimulationButton.interactable = false;
    }

    public void EnableDeleteSimulationButton()
    {
        _deleteSimulationButton.interactable = true;
    }

    public void DisableDeleteSimulationButton()
    {
        _deleteSimulationButton.interactable = false;
    }

    public void EnableSimulationButton()
    {
        _simulationButton.interactable = true;
    }

    public void DisableSimulationButton()
    {
        _simulationButton.interactable = false;
    }

    // Dialogs
    public void ShowDialog(GameObject dialog)
    {
        DisableWorldInteraction();
        
        backdrop.GetComponent<CanvasGroup>().alpha = 0f;
        backdrop.SetActive(true);

        LeanTween.alphaCanvas(backdrop.GetComponent<CanvasGroup>(), 1f, 1f);
        
        var slide = dialog.GetComponent<SlideUI>();
        slide.SetVisible(true);
    }

    public void HideDialog(GameObject dialog)
    {
        var slide = dialog.GetComponent<SlideUI>();
        slide.SetVisible(false);
        
        LeanTween.alphaCanvas(backdrop.GetComponent<CanvasGroup>(), 0f, 1f).setOnComplete(() =>
        {
            backdrop.SetActive(false);
            EnableWorldInteraction();
        });
    }

    public void ShowConnectionsDialog()
    {
        connectionsDialog.PopulateForm();
        ShowDialog(connectionsDialog.gameObject);
    }

    public void HideConnectionsDialog()
    {
        HideDialog(connectionsDialog.gameObject);
    }
}