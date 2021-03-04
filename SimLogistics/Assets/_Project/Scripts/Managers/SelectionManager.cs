using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private BoolVariable isWorldInteractable;
    [SerializeField] private new Camera camera;
    [SerializeField] private int layer;
    [SerializeField] private float maxHitDistance = 1000f;

    private SelectableObject _curSelectableObject;
    private readonly List<SelectableObject> _curSelectedObjects = new List<SelectableObject>();
    
    private void Update()
    {
        if (Pointer.IsOverUIObject())
        {
            Leave();
            return;
        }
        
        if (!isWorldInteractable.Value) return;
        
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hitInfo, maxHitDistance, 1 << layer))
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

        var isMouseDown = Input.GetMouseButtonDown(0);

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

        print($"[name] Leave {_curSelectableObject.name}");
        _curSelectableObject.onHoverLeave.Invoke();

        _curSelectableObject = null;
    }

    private void Select()
    {
        _curSelectedObjects.Add(_curSelectableObject);
        _curSelectableObject.onSelect.Invoke();
    }
    
    private void Deselect()
    {
        _curSelectableObject.onDeselect.Invoke();
        _curSelectedObjects.Remove(_curSelectableObject);
        Enter();
    }
}
