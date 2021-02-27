using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Title : MonoBehaviour
{
    [SerializeField] private UnityEvent onTitlesComplete;

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject logo;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float fadeDelay;
    
    public void Roll()
    {
        LeanTween.alphaCanvas(title.GetComponent<CanvasGroup>(), 1f, fadeDuration).setDelay(fadeDelay).setOnComplete(OnTitleComplete);
    }
    
    private void OnTitleComplete()
    {
        LeanTween.alphaCanvas(logo.GetComponent<CanvasGroup>(), 1f, fadeDuration).setOnComplete(OnLogoComplete);
    }
    
    private void OnLogoComplete()
    {
        LeanTween.alphaCanvas(title.GetComponent<CanvasGroup>(), 0f, fadeDuration).setDelay(fadeDelay);
        LeanTween.alphaCanvas(logo.GetComponent<CanvasGroup>(), 0f, fadeDuration).setDelay(fadeDelay);
        
        onTitlesComplete.Invoke();
    }
}
