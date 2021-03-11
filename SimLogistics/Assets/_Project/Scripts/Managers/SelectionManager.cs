using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private BoolVariable isWorldInteractable;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private int layer;
    [SerializeField] private float maxHitDistance = 1000f;
    [SerializeField] private int maxSelectedObjects = 2;

    private SelectableObject _curSelectableObject;
    private readonly List<SelectableObject> _curSelectedObjects = new List<SelectableObject>();

    public IEnumerable<SelectableObject> SelectedObjects => _curSelectedObjects;

    public void OnLoaded()
    {
        _curSelectedObjects.Clear();
    }

    private void Update()
    {
        if (!isWorldInteractable.Value) return;

        var isMouseDown = Input.GetMouseButtonDown(0);

        var ray = cameraController.Camera.ScreenPointToRay(Input.mousePosition);
        var isHit = Physics.Raycast(ray, out var hitInfo, maxHitDistance, 1 << layer);

        cameraController.HandleInput(ray, isHit);

        if (cameraController.IsDragging || Pointer.IsOverUIObject())
        {
            Leave();
            return;
        }

        if (!isHit)
        {
            Leave();
            return;
        }

        var hitObject = hitInfo.collider.gameObject;

        var selectableObject = hitObject.GetComponent<SelectableObject>();
        if (selectableObject == null)
        {
            Leave();
            return;
        }

        if (selectableObject != _curSelectableObject)
        {
            Leave();
        }
        else
        {
            if (!isMouseDown) return;
        }

        _curSelectableObject = selectableObject;

        var isCurSelected = _curSelectedObjects.Contains(_curSelectableObject);
        if (!isCurSelected)
        {
            Enter();
        }

        if (!isMouseDown) return;

        if (isCurSelected)
        {
            Deselect(_curSelectableObject);
        }
        else
        {
            Select();
        }
    }

    private void Enter()
    {
        _curSelectableObject.onHoverEnter.Invoke();
    }

    private void Leave()
    {
        if (_curSelectableObject == null) return;
        if (_curSelectedObjects.Contains(_curSelectableObject)) return;

        _curSelectableObject.onHoverLeave.Invoke();

        _curSelectableObject = null;
    }

    private void Select()
    {
        var isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isShift)
        {
            if (_curSelectedObjects.Count == maxSelectedObjects)
            {
                Deselect(_curSelectedObjects[maxSelectedObjects - 1]);
            }
        }
        else
        {
            DeselectAll();
        }
        
        _curSelectedObjects.Add(_curSelectableObject);
        _curSelectableObject.onSelect.Invoke();
    }

    private void Deselect(SelectableObject selectableObject)
    {
        selectableObject.onDeselect.Invoke();
        if (_curSelectedObjects.Count == 0) return;

        _curSelectedObjects.Remove(selectableObject);
        Enter();
    }

    public void DeselectAll()
    {
        foreach (var selectedObject in _curSelectedObjects)
        {
            selectedObject.onDeselect.Invoke();
        }

        _curSelectedObjects.Clear();
    }
}