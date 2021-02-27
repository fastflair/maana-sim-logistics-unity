using System;
using UnityEngine;

[CreateAssetMenu]
public class StringVariable : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private string defaultValue;

    [NonSerialized] public string Value;

    public void OnBeforeSerialize()
    {
        Value = defaultValue;
    }

    public void OnAfterDeserialize() { }
}
