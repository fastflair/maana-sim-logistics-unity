using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onBootstrap;
    [SerializeField] private BoolVariable isWorldInteractable;
    
    private void Start()
    {
        DisableWorldInteraction();
        onBootstrap.Invoke();
    }

    public void EnableWorldInteraction()
    {
        isWorldInteractable.Value = true;
    }
    
    public void DisableWorldInteraction()
    {
        isWorldInteractable.Value = false;
    }
}
