using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    // Settings
    [SerializeField] private new Camera camera;
    [SerializeField] private BoolVariable isWorldInteractable;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTime;
    [SerializeField] private float rotationAmount;
    [SerializeField] private float mouseRotationSpeed;
    [SerializeField] private Vector3 zoomAmount;
    [SerializeField] private Vector3 minBoundZoom;
    [SerializeField] private Vector3 maxBoundZoom;
    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable mapTilesX;
    [SerializeField] private FloatVariable mapTilesY;

    // Changes
    private Vector3 _newPosition;
    private Quaternion _newRotation;
    private Vector3 _newZoom;

    // Mouse
    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;
    private Vector3 _rotateStartPosition;
    private Vector3 _rotateCurrentPosition;
    
    private void Start()
    {
        _newPosition = transform.position;
        _newRotation = transform.rotation;
        _newZoom = camera.transform.localPosition;
    }
    
    private void Update()
    {
        if (!isWorldInteractable.Value) return;

        HandleMovement();
    }
    
    private void HandleMovement()
    {
        HandleInputKeyboard();
        HandleInputMouse();
        
        ClampPosition();
        ClampZoom();
        
        transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, Time.deltaTime * movementTime);
        camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, _newZoom, Time.deltaTime * movementTime);
    }

    private void HandleInputMouse()
    {
        HandleInputMousePosition();
        HandleInputMouseRotation();
        HandleInputMouseZoom();
    }
    
    private void HandleInputMousePosition()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GetDragPoint(out var point))
            {
                _dragStartPosition = point;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (GetDragPoint(out var point))
            {
                _dragCurrentPosition = point;
                _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
            }
        }
    }
    
    private void HandleInputMouseRotation()
    {
        if (Input.GetMouseButtonDown(2))
        {
            _rotateStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            _rotateCurrentPosition = Input.mousePosition;
            var delta = _rotateCurrentPosition - _rotateStartPosition;
            _newRotation *= quaternion.Euler(Vector3.up * (-delta.x/mouseRotationSpeed));
            _rotateStartPosition = _rotateCurrentPosition;
        }
    }

    private void HandleInputMouseZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            _newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }
    }

    private void HandleInputKeyboard()
    {
        HandleInputKeyboardPosition();
        HandleInputKeyboardRotation();
        HandleInputKeyboardZoom();
    }

    private void HandleInputKeyboardPosition()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _newPosition += transform.forward * movementSpeed;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _newPosition += transform.forward * -movementSpeed;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _newPosition += transform.right * movementSpeed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _newPosition += transform.right * -movementSpeed;
        }
    }
    
    private void HandleInputKeyboardRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
    }

    private void HandleInputKeyboardZoom()
    {
        if (Input.GetKey(KeyCode.R))
        {
            _newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            _newZoom -= zoomAmount;
        }
    }
    
    private bool GetDragPoint(out Vector3 point)
    {
        var plane = new Plane(Vector3.up, Vector3.zero); 
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out var entry))
        {
            point = ray.GetPoint(entry);
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    private void ClampPosition()
    {
        var maxX = tileSize.Value * mapTilesX.Value;
        var maxZ = -tileSize.Value * mapTilesY.Value;

        // print($"Cam.Pos: {transform.position} - clamp: {maxX}, {maxZ}");

        _newPosition.x = Mathf.Clamp(_newPosition.x, 0, maxX);
        _newPosition.z = Mathf.Clamp(_newPosition.z, maxZ, 0);
    }

    private void ClampZoom()
    {
        _newZoom.y = Mathf.Clamp(_newZoom.y, minBoundZoom.y, maxBoundZoom.y);
        _newZoom.z = Mathf.Clamp(_newZoom.z, minBoundZoom.z, maxBoundZoom.z);
    }
}
