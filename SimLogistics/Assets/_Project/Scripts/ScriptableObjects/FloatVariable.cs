using System;
using UnityEngine;

[CreateAssetMenu]
public class FloatVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public float defaultValue;

    [NonSerialized] public float Value;

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        Value = defaultValue;
    }
}
