using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QEntityTraversalCompatibilityFragment
{
    public const string data = @"
        fragment entityTraversalCompatibilityData on EntityTraversalCompatibility {
          id
          entityType
          traversalType
        }";
}

public class QEntityTraversalCompatibility
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string entityType;
    [UsedImplicitly] public string traversalType;

    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            entityType: ""{entityType}""
            traversalType: ""{traversalType}""
        }}";
    }
}