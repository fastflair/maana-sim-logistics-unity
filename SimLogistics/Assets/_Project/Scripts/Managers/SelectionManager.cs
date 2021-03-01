using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private BoolVariable isWorldInteractable;
    [SerializeField] private new Camera camera;
    [SerializeField] private int layer;

    private SelectableObject _curSelectableObject;
    private GameObject _curHitObject;
    private bool _hasCurrent;
    
    private void Update()
    {
        if (Pointer.IsOverUIObject()) return;
        
        if (!isWorldInteractable.Value) return;
        
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hitInfo, 100f, 1 << layer))
        {
            Exit();
            return;
        }

        var hitObject = hitInfo.collider.gameObject;
        if (_hasCurrent && hitObject == _curHitObject)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Select();
            }

            return;
        }
        
        var selectableObject = hitObject.GetComponent<SelectableObject>();
        if (selectableObject == null)
        {
            Exit();
            return;
        }
        
        Enter(selectableObject, hitObject);
    }
    
    private void Enter(SelectableObject selectableObject, GameObject hitObject)
    {
        _curSelectableObject = selectableObject;
        _curHitObject = hitObject;
        _hasCurrent = true;
        selectableObject.SelectionHandler.OnHoverEnter(hitObject);
    }
    
    private void Exit()
    {
        if (!_hasCurrent) return;
            
        _curSelectableObject.SelectionHandler.OnHoverExit(_curHitObject);

        _curSelectableObject = null;
        _curHitObject = null;
        _hasCurrent = false;
    }

    private void Select()
    {
        _curSelectableObject.SelectionHandler.OnSelect(_curHitObject);
    }
}
