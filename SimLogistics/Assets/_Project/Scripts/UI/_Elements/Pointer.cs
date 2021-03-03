using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    public static bool IsOverUIObject()
    {
        var ed = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        var res = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ed,res);
        return res.Count > 0;
    }
}
