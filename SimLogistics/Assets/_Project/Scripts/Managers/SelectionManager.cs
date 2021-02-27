using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private int layer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hitInfo, 1000f, 1 << layer)) return;

        var hitObject = hitInfo.collider.gameObject;
        var selectableObject = hitObject.GetComponent<SelectableObject>();
        if (selectableObject == null) return;
        print("hit: " + hitObject.name + " " + selectableObject);
        selectableObject.SelectionHandler.OnHoverEnter(hitObject);
        print(hitInfo.collider.gameObject.name);
    }
}
