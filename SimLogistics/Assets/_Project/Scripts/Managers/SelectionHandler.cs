using UnityEngine;

public interface ISelectionHandler
{
    void OnHoverEnter(GameObject selectedObject);
    void OnHoverExit(GameObject selectedObject);
    void OnSelect(GameObject selectedObject);
    void OnDeselect(GameObject selectedObject);
}