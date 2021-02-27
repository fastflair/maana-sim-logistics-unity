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
    [SerializeField] private float focusSpeed;
    [SerializeField] private int excludeLayer;
    
    private Ray _raycast;
    private RaycastHit _hit;
    private float _hitDistance;
    private DepthOfField _dof;
    
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
        var localTransform = transform;
        var position = localTransform.position;
        
        _raycast = new Ray(position, localTransform.forward * maxFocusDistance);
        _hitDistance = Physics.Raycast(_raycast, out _hit, maxFocusDistance, ~(excludeLayer << 8)) ? Vector3.Distance(position, _hit.point) : maxFocusDistance;

        SetFocus();
    }

    private void SetFocus()
    {
        _dof.focusDistance.value = Mathf.Lerp(_dof.focusDistance.value, _hitDistance, Time.deltaTime * focusSpeed);
    }
}
