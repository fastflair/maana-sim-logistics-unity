using System;
using UnityEngine;

[CreateAssetMenu]
public class StringVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public string defaultValue;

    [NonSerialized] public string Value;

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        Value = defaultValue;
    }
}
