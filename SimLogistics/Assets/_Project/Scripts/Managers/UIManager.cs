using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onBootstrap;
    [SerializeField] private BoolVariable isWorldInteractable;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject hud;
    [SerializeField] private float hudFadeInDuration;
    [SerializeField] private float hudFadeInDelay;
    [SerializeField] private float hudFadeOutDuration;
    [SerializeField] private float hudFadeOutDelay;
    
    private void Start()
    {
        DisableWorldInteraction();
        // HideHUD();
        ShowTitle();
        onBootstrap.Invoke();
    }

    public void EnableWorldInteraction()
    {
        isWorldInteractable.Value = true;
    }
    
    public void DisableWorldInteraction()
    {
        isWorldInteractable.Value = false;
    }

    public void ShowTitle()
    {
        title.SetActive(true);
    }

    public void HideTitle()
    {
        title.SetActive(false);
    }
    
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
}
