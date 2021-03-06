using UnityEngine;
using UnityEngine.Events;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private UnityEvent onLoadingScreenComplete;

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject background;
    [SerializeField] private float titleFadeDuration;
    [SerializeField] private float titleFadeDelay;
    [SerializeField] private float endFadeDuration;
    [SerializeField] private float endFadeDelay;

    private bool _isTitlesComplete;
    private bool _isLoaded;
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnLoading()
    {        
        background.GetComponent<CanvasGroup>().alpha = 1f;
        background.SetActive(true);
        title.GetComponent<CanvasGroup>().alpha = 0f;
        logo.GetComponent<CanvasGroup>().alpha = 0f;
    
        gameObject.SetActive(true);
        LeanTween.alphaCanvas(title.GetComponent<CanvasGroup>(), 1f, titleFadeDuration).setDelay(titleFadeDelay)
            .setOnComplete(OnTitleFadeComplete);
    }

    public void OnLoaded()
    {
        _isLoaded = true;

        FadeOut();
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void OnTitleFadeComplete()
    {
        LeanTween.alphaCanvas(logo.GetComponent<CanvasGroup>(), 1f, titleFadeDuration).setOnComplete(OnLogoFadeComplete);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnLogoFadeComplete()
    {
        _isTitlesComplete = true;

        FadeOut();
    }
    
    private void FadeOut()
    {
        if (!_isLoaded || !_isTitlesComplete) return;
        
        LeanTween.alphaCanvas(background.GetComponent<CanvasGroup>(), 0f, endFadeDuration).setDelay(endFadeDelay)
            .setOnComplete(OnFadeOutComplete);
    }

    private void OnFadeOutComplete()
    {
        _isLoaded = _isTitlesComplete = false;
        background.SetActive(false);
        onLoadingScreenComplete.Invoke();
    }
}