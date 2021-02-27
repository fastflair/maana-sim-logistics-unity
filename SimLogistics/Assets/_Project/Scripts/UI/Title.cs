using UnityEngine;
using UnityEngine.Events;

public class Title : MonoBehaviour
{
    [SerializeField] private UnityEvent onTitlesComplete;

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject background;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float fadeDelay;

    public void Start()
    {
        background.SetActive(true);
        background.GetComponent<CanvasGroup>().alpha = 1f;
        title.GetComponent<CanvasGroup>().alpha = 0f;
        logo.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void Roll()
    {
        LeanTween.alphaCanvas(title.GetComponent<CanvasGroup>(), 1f, fadeDuration).setDelay(fadeDelay)
            .setOnComplete(OnTitleComplete);
    }

    private void OnTitleComplete()
    {
        LeanTween.alphaCanvas(logo.GetComponent<CanvasGroup>(), 1f, fadeDuration).setOnComplete(OnLogoComplete);
    }

    private void OnLogoComplete()
    {
        LeanTween.alphaCanvas(logo.GetComponent<CanvasGroup>(), 0f, fadeDuration).setDelay(fadeDelay);
        LeanTween.alphaCanvas(title.GetComponent<CanvasGroup>(), 0f, fadeDuration).setDelay(fadeDelay)
            .setOnComplete(OnTitleFadeOutComplete);
    }

    private void OnTitleFadeOutComplete()
    {
        LeanTween.alphaCanvas(background.GetComponent<CanvasGroup>(), 0f, fadeDuration).setDelay(fadeDelay)
            .setOnComplete(OnFadeOutComplete);
    }

    private void OnFadeOutComplete()
    {
        onTitlesComplete.Invoke();
    }
}