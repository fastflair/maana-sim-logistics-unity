using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SlideUI : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    // Keep this to 0 if you want this element to perfectly align to the edge of the screen (when off screen).
    private const float Overshoot = .1f;

    [SerializeField] private Direction slideToScreenEdge;

    [SerializeField] private float animDuration = .3f;

    [SerializeField] private bool visibleOnStart = true;

    [SerializeField] private bool delayedShow = true;

    private bool _isVisible;
    private Vector2 _offsetDirection = Vector2.zero;
    private RectTransform _rt, _canvasRT;
    private Vector2 _visiblePos, _offScreenPos;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _canvasRT = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        _visiblePos = _rt.anchoredPosition;
        _isVisible = !visibleOnStart;
        SetDirection(slideToScreenEdge);
        SetVisible(visibleOnStart, false);
    }


    public bool GetVisible()
    {
        return _isVisible;
    }

    public void SetVisible(bool onOff, bool isAnimated = true)
    {
        if (_isVisible == onOff) return;

        _isVisible = onOff;
        if (isAnimated)
        {
            var tween = LeanTween.move(_rt, _isVisible ? _visiblePos : _offScreenPos, animDuration);
            tween.setEase(_isVisible ? LeanTweenType.easeOutExpo : LeanTweenType.easeInExpo);
            // Delay is set so that these tabs can get "swiped".
            if (delayedShow && onOff)
                tween.setDelay(animDuration);
            return;
        }

        _rt.anchoredPosition = _isVisible ? _visiblePos : _offScreenPos;
    }

    public void SetDirection(Direction direction)
    {
        var dirVector = Vector2.zero;
        var canvasSize = _canvasRT.sizeDelta;
        var corners = new Vector3 [4];


        // Calculate four corners of our rect transform
        _rt.GetWorldCorners(corners);
        for (var i = 0; i < 4; i++)
        {
            corners[i] = _canvasRT.InverseTransformPoint(corners[i]);
        }

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
        }
    }
}