using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField] private float hudFadeInDuration;
    [SerializeField] private float hudFadeInDelay;
    [SerializeField] private float hudFadeOutDuration;
    [SerializeField] private float hudFadeOutDelay;

    // HUD: Command Bars and Buttons
    [SerializeField] private GameObject systemCommandBar;
    [SerializeField] private GameObject simulationCommandBar;
    [SerializeField] private Button newButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button simulationButton;

    // Dialogs
    [SerializeField] private GameObject backdrop;
    [SerializeField] private GameObject connectionsDialog;

    private void Start()
    {
        // Initial conditions
        DisableWorldInteraction();
        HideHUD();
        SimulationNotReady();
        ShowSystemCommandBar();
        HideConnectionsDialog();

        ShowTitle();

        onBootstrap.Invoke();
    }

    public void SimulationReady()
    {
        EnableNewButton();
        EnableLoadButton();
        EnableDeleteButton();
        EnableSimulationButton();
    }

    public void SimulationNotReady()
    {
        DisableNewButton();
        DisableLoadButton();
        DisableDeleteButton();
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
        LeanTween.alphaCanvas(hud.GetComponent<CanvasGroup>(), 1f, hudFadeInDuration).setDelay(hudFadeInDelay);
    }

    public void FadeHUDOut()
    {
        hud.GetComponent<CanvasGroup>().alpha = 1f;
        LeanTween.alphaCanvas(hud.GetComponent<CanvasGroup>(), 0f, hudFadeOutDuration).setDelay(hudFadeOutDelay)
            .setOnComplete(HideHUD);
        ;
    }

    public void ShowHUD()
    {
        hud.SetActive(true);
    }

    public void HideHUD()
    {
        hud.SetActive(false);
    }

    // HUD: Command Bars
    public void ShowSystemCommandBar()
    {
        HideSimulationCommandBar();
        systemCommandBar.SetActive(true);
    }

    public void HideSystemCommandBar()
    {
        systemCommandBar.SetActive(false);
    }

    public void ShowSimulationCommandBar()
    {
        HideSystemCommandBar();
        simulationCommandBar.SetActive(true);
    }

    public void HideSimulationCommandBar()
    {
        simulationCommandBar.SetActive(false);
    }

    public void EnableNewButton()
    {
        newButton.interactable = true;
    }

    public void DisableNewButton()
    {
        newButton.interactable = false;
    }

    public void EnableLoadButton()
    {
        loadButton.interactable = true;
    }

    public void DisableLoadButton()
    {
        loadButton.interactable = false;
    }

    public void EnableDeleteButton()
    {
        deleteButton.interactable = true;
    }

    public void DisableDeleteButton()
    {
        deleteButton.interactable = false;
    }

    public void EnableSimulationButton()
    {
        simulationButton.interactable = true;
    }

    public void DisableSimulationButton()
    {
        simulationButton.interactable = false;
    }

    // Dialogs
    public void ShowDialog(GameObject dialog)
    {
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
        LeanTween.alphaCanvas(backdrop.GetComponent<CanvasGroup>(), 0f, 1f).setOnComplete(() => backdrop.SetActive(false));
    }

    public void ShowConnectionsDialog()
    {
        ShowDialog(connectionsDialog);
    }

    public void HideConnectionsDialog()
    {
        HideDialog(connectionsDialog);
    }
}