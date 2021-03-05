using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public class QEntity
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string sim;
    [UsedImplicitly] public int steps;
    [UsedImplicitly] public float x;
    [UsedImplicitly] public float y;

    public override string ToString()
    {
        return $"QEntity {{ id: {id}, sim: {sim}, steps: {steps}, x: {x}, y: {y} }}";
    }
}