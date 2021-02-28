using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Events
    [SerializeField] private UnityEvent onBootstrap;
    
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

    private void Start()
    {
        // Initial conditions
        DisableWorldInteraction();
        HideHUD();
        SimulationNotReady();
        ShowSystemCommandBar();
                
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
        LeanTween.alphaCanvas(hud.GetComponent<CanvasGroup>(), 0f, hudFadeOutDuration).setDelay(hudFadeOutDelay).setOnComplete(HideHUD);;
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
}
