using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthOfFieldController : MonoBehaviour
{
    [SerializeField] private GameObject volumeObject;
    // [SerializeField] private float minFocusDistance;//= .07f;
    // [SerializeField] private float minFocalLength;// = 50f;
    [SerializeField] private float maxFocusDistance = 14.0f;
    // [SerializeField] private float maxFocalLength = 300f;

    
    private Ray _raycast;
    private RaycastHit _hit;
    private bool _isHit;
    private float _hitDistance;
    private DepthOfField _dof;

    private void Start()
    {
        var v = volumeObject.GetComponent<Volume>();

        if (v.profile.TryGet(out DepthOfField tmp))
        {
            _dof = tmp;
        }
        
        // No DOF at start (during titles)
        // LeanTween.value(gameObject, 0f, maxFocalLength, 5f).setOnUpdate( val=>{
        //     _dof.focalLength.value = val;
        // } );
        
        LeanTween.value(gameObject, 0f, maxFocusDistance, 5f).setOnUpdate( val=>{ 
            _dof.focusDistance.value = val;
        } );
    }

    private void Update()
    {
        _raycast = new Ray(transform.position, transform.forward * 100);
        _isHit = false;

        if (Physics.Raycast(_raycast, out _hit, 100f))
        {
            _isHit = true;
            _hitDistance = Vector3.Distance(transform.position, _hit.point);
            // print("hit: " + _hit.collider.gameObject.name);
        }
        else
        {
            if (_hitDistance < 100f)
            {
                _hitDistance++;
            }
        }

        SetFocus();
    }

    private void SetFocus()
    {
        _dof.focusDistance.value =_hitDistance;
        // _dof.focalLength.value = minFocalLength + zoom * (maxFocalLength - minFocalLength);
    }
    
    private void OnDrawGizmos()
    {
        if (_isHit)
        {
            Gizmos.DrawSphere(_hit.point, 0.1f);
            Debug.DrawRay(transform.position, transform.forward * Vector3.Distance(transform.position, _hit.point));
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 100f);
        }
    }
}
