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
    private bool _isHit;
    private float _hitDistance;
    private DepthOfField _dof;
    private Transform _transform;
    
    private void Start()
    {
        // cache
        _transform = transform;
        
        var v = volumeObject.GetComponent<Volume>();

        if (v.profile.TryGet(out DepthOfField tmp))
        {
            _dof = tmp;
        }
        
        // No DOF at start (during titles)
        LeanTween.value(gameObject, 0f, maxFocusDistance, 5f).setOnUpdate( val=>{ 
            _dof.focusDistance.value = val;
        } );
    }

    private void Update()
    {
        _raycast = new Ray(transform.position, _transform.forward * maxFocusDistance);
        _isHit = false;

        if (Physics.Raycast(_raycast, out _hit, maxFocusDistance, ~(excludeLayer << 8)))
        {
            _isHit = true;
            _hitDistance = Vector3.Distance(transform.position, _hit.point);
        }
        else
        {
            if (_hitDistance < maxFocusDistance)
            {
                _hitDistance++;
            }
        }

        SetFocus();
    }

    private void SetFocus()
    {
        _dof.focusDistance.value = Mathf.Lerp(_dof.focusDistance.value, _hitDistance, Time.deltaTime * focusSpeed);
    }
    
    private void OnDrawGizmos()
    {
        if (_isHit)
        {
            Gizmos.DrawSphere(_hit.point, 0.1f);
            Debug.DrawRay(transform.position, _transform.forward * Vector3.Distance(transform.position, _hit.point));
        }
        else
        {
            Debug.DrawRay(transform.position, _transform.forward * 100f);
        }
    }
}
