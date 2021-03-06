using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private BoolVariable isWorldInteractable;
    [SerializeField] private new Camera camera;
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
        if (Pointer.IsOverUIObject())
        {
            Leave();
            return;
        }
        
        if (!isWorldInteractable.Value) return;
        
        var isMouseDown = Input.GetMouseButtonDown(0);

        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hitInfo, maxHitDistance, 1 << layer))
        {
            if (isMouseDown) DeselectAll();
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
            Deselect();
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
        if (_curSelectedObjects.Count == maxSelectedObjects) return;
        
        _curSelectedObjects.Add(_curSelectableObject);
        _curSelectableObject.onSelect.Invoke();
    }
    
    private void Deselect()
    {
        if (_curSelectedObjects.Count == 0) return;
        
        _curSelectableObject.onDeselect.Invoke();
        _curSelectedObjects.Remove(_curSelectableObject);
        Enter();
    }
    
    private void DeselectAll()
    {
        foreach (var selectedObject in _curSelectedObjects)
        {
            selectedObject.onDeselect.Invoke();
        }
        _curSelectedObjects.Clear();
    }
}
