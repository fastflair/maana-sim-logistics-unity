using UnityEngine;

public interface ISelectionHandler
{
    void OnHoverEnter(GameObject obj);
    void OnHoverExit(GameObject obj);
    void OnSelect(GameObject selectedObject);
}