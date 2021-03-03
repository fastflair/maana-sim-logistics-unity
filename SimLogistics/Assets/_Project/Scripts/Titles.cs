using UnityEngine;
using UnityEngine.Events;

public class Titles : MonoBehaviour
{
    [SerializeField] private UnityEvent titlesCompleteEvent;

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject background;
    [SerializeField] private float titleFadeDuration;
    [SerializeField] private float titleFadeDelay;
    [SerializeField] private float endFadeDuration;
    [SerializeField] private float endFadeDelay;

    public void Start()
    {
        // // Skip title sequence
        // titlesCompleteEvent.Invoke();
        // return;

        background.GetComponent<CanvasGroup>().alpha = 1f;
        background.SetActive(true);
        title.GetComponent<CanvasGroup>().alpha = 0f;
        logo.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Roll()
    {
        gameObject.SetActive(true);
        LeanTween.alphaCanvas(title.GetComponent<CanvasGroup>(), 1f, titleFadeDuration).setDelay(titleFadeDelay)
            .setOnComplete(OnTitleComplete);
    }

    private void OnTitleComplete()
    {
        LeanTween.alphaCanvas(logo.GetComponent<CanvasGroup>(), 1f, titleFadeDuration).setOnComplete(OnLogoComplete);
    }

    private void OnLogoComplete()
    {
        LeanTween.alphaCanvas(background.GetComponent<CanvasGroup>(), 0f, endFadeDuration).setDelay(endFadeDelay)
            .setOnComplete(OnFadeOutComplete);
    }

    private void OnFadeOutComplete()
    {
        background.SetActive(false);
        titlesCompleteEvent.Invoke();
    }
}