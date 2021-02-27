using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/Int")]
public class IntVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public int defaultValue;

    [NonSerialized] public int Value;

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        Value = defaultValue;
    }
}
