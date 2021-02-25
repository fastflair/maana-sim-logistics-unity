using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject logo;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float fadeDelay;
    
    private void Start()
    {
        LeanTween.alphaCanvas(title.GetComponent<CanvasGroup>(), 1f, fadeDuration).setDelay(fadeDelay).setOnComplete(OnTitleComplete);
    }

    private void OnTitleComplete()
    {
        print("Title complete");
        LeanTween.alphaCanvas(logo.GetComponent<CanvasGroup>(), 1f, fadeDuration).setOnComplete(OnLogoComplete);
    }
    
    private void OnLogoComplete()
    {
        print("Logo complete");
        
        LeanTween.alphaCanvas(title.GetComponent<CanvasGroup>(), 0f, fadeDuration).setDelay(fadeDelay);
        LeanTween.alphaCanvas(logo.GetComponent<CanvasGroup>(), 0f, fadeDuration).setDelay(fadeDelay);
    }
}
