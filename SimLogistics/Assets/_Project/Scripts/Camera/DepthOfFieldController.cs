using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthOfFieldController : MonoBehaviour
{
    [SerializeField] private GameObject volumeObject;
    [SerializeField] private float maxFocusDistance;
    [SerializeField] private float blurFocusDistance = 0.1f;
    [SerializeField] private float focusSpeed;
    [SerializeField] private int layer;
    
    private Ray _raycast;
    private RaycastHit _hit;
    private float _hitDistance;
    private DepthOfField _dof;
    private bool _isBlurred;
    
    private void Start()
    {
        var volume = volumeObject.GetComponent<Volume>();

        if (!volume.profile.TryGet(out _dof))
        {
            throw new Exception("Depth of field not found in volume profile");
        }
    }

    private void Update()
    {
        if (_isBlurred)
        {
            SetFocus(blurFocusDistance);
            return;
        }

        var localTransform = transform;
        var position = localTransform.position;
        
        _raycast = new Ray(position, localTransform.forward * maxFocusDistance);
        _hitDistance = Physics.Raycast(_raycast, out _hit, maxFocusDistance, 1 << layer) ? Vector3.Distance(position, _hit.point) : maxFocusDistance;

        SetFocus(_hitDistance);
    }

    private void SetFocus(float focusDistance)
    {
        _dof.focusDistance.value = Mathf.Lerp(_dof.focusDistance.value, focusDistance, Time.deltaTime * focusSpeed);
    }

    public void BlurBackground()
    {
        _isBlurred = true;
    }

    public void UnblurBackground()
    {
        _isBlurred = false;
    }
}
