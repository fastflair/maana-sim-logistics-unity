using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Bool")]
public class BoolVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public bool defaultValue;

    [NonSerialized] public bool Value;

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        Value = defaultValue;
    }
}
