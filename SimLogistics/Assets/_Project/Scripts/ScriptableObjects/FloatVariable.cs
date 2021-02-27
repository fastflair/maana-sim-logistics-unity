using System;
using UnityEngine;

[CreateAssetMenu]
public class FloatVariable : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private float defaultValue;

    [NonSerialized] public float Value;

    public void OnBeforeSerialize()
    {
        Value = defaultValue;
    }

    public void OnAfterDeserialize() { }
}
