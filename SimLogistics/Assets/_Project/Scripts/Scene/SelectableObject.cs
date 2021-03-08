using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectableObject : MonoBehaviour
{
    public UnityEvent onHoverEnter = new UnityEvent();
    public UnityEvent onHoverLeave = new UnityEvent();
    public UnityEvent onSelect = new UnityEvent();
    public UnityEvent onDeselect = new UnityEvent();

    [SerializeField] private GameObject meshRendererSource;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color selectColor;

    private MeshRenderer _meshRenderer;
    private Color _normalColor;
    
    private void Start()
    {
        _meshRenderer = meshRendererSource.GetComponent<MeshRenderer>();
        _normalColor = _meshRenderer.material.color;
        
        onHoverEnter.AddListener(Highlight);
        onHoverLeave.AddListener(Unhighlight);
        onSelect.AddListener(Select);
        onDeselect.AddListener(Deselect);
    }

    private void Highlight()
    {
        _meshRenderer.material.color = highlightColor;
    }

    private void Unhighlight()
    {
        _meshRenderer.material.color = _normalColor;
    }

    private void Select()
    {
        _meshRenderer.material.color = selectColor;
    }

    private void Deselect()
    {
        _meshRenderer.material.color = _normalColor;
    }
}
