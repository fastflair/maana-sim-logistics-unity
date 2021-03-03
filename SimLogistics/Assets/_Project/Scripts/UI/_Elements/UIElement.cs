using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class UIElement : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum Effect
    {
        None,
        Fade,
        Animate
    }

    // Keep this to 0 if you want this element to perfectly align to the edge of the screen (when off screen).
    private const float Overshoot = .1f;

    [SerializeField] private bool visibleOnStart = false;
    [SerializeField] private Direction slideToScreenEdge = Direction.Down;
    [SerializeField] private float animDuration = .3f;
    [SerializeField] private float animDelay = .3f;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float fadeDelay = 2f;

    public bool IsVisible { get; private set; }

    private Vector2 _visiblePos;
    private Vector2 _offScreenPos;
    
    private void Awake()
    {
        var rt = GetComponent<RectTransform>();
        _visiblePos = rt.anchoredPosition;
        
        SetDirection(slideToScreenEdge);

        IsVisible = !visibleOnStart;
        SetVisible(visibleOnStart);
    }
    
    public LTDescr SetVisible(bool isVisible, Effect effect = Effect.None)
    {
        var rt = GetComponent<RectTransform>();
        // print($"SetVisible: {rt} {isVisible} {effect}");

        if (isVisible)
        {
            gameObject.SetActive(true);
        }
        
        if (IsVisible == isVisible)
        {
            return null;
        }


        LTDescr tween = null;

        switch (effect)
        {
            case Effect.Animate:
            {
                Vector3 toPos;
                LeanTweenType easeType;

                if (isVisible)
                {
                    toPos = _visiblePos;
                    easeType = LeanTweenType.easeOutExpo;
                }
                else
                {
                    toPos = _offScreenPos;
                    easeType = LeanTweenType.easeInExpo;
                }
                
                tween = LeanTween
                    .move(rt, toPos, animDuration)
                    .setEase(easeType)
                    .setDelay(animDelay);
                
                break;
            }
            case Effect.Fade:
            {
                float fadeFrom;
                float fadeTo;
                    
                if (isVisible)
                {
                    fadeFrom = 0f;
                    fadeTo = 1f;
                }
                else
                {
                    fadeFrom = 1f;
                    fadeTo = 0f;
                }

                var canvasGroup = GetComponent<CanvasGroup>();
                canvasGroup.alpha = fadeFrom;
                
                tween = LeanTween
                    .alphaCanvas(canvasGroup, fadeTo, fadeDuration)
                    .setDelay(fadeDelay);
                break;
            }
            case Effect.None:
            {
                gameObject.SetActive(isVisible);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
        }

        IsVisible = isVisible;
        
        if (effect == Effect.Animate) return tween;
        
        rt.anchoredPosition = IsVisible ? _visiblePos : _offScreenPos;

        return tween;
    }

    public void SetDirection(Direction direction)
    {
        var rt = GetComponent<RectTransform>();
        var canvasRT = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        
        var canvasSize = canvasRT.sizeDelta;
        var corners = new Vector3 [4];
        
        // Calculate four corners of our rect transform
        rt.GetWorldCorners(corners);
        for (var i = 0; i < 4; i++) corners[i] = canvasRT.InverseTransformPoint(corners[i]);

        float offScreenValue;
        switch (direction)
        {
            case Direction.Up:
                offScreenValue = (.5f + Overshoot) * canvasSize.y - corners[0].y;
                _offScreenPos.Set(_visiblePos.x, _visiblePos.y + offScreenValue);
                break;
            case Direction.Down:
                offScreenValue = -(.5f + Overshoot) * canvasSize.y - corners[1].y;
                _offScreenPos.Set(_visiblePos.x, _visiblePos.y + offScreenValue);
                break;
            case Direction.Left:
                offScreenValue = -(.5f + Overshoot) * canvasSize.x - corners[2].x;
                _offScreenPos.Set(_visiblePos.x + offScreenValue, _visiblePos.y);
                break;
            case Direction.Right:
                offScreenValue = (.5f + Overshoot) * canvasSize.x - corners[0].x;
                _offScreenPos.Set(_visiblePos.x + offScreenValue, _visiblePos.y);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
}