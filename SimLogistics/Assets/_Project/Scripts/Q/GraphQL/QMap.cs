using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QMapFragment
{
    public const string data = @"
        fragment mapData on Map {
          id
          tilesX
          tilesY
          tileDescriptors { ...tileDescriptorData }
          entityTraversalCompatibilities { ...entityTraversalCompatibilityData }
        }";

    public static readonly string withIncludes = @$"
      {QTileDescriptorFragment.data}
      {QEntityTraversalCompatibilityFragment.data}
      {data}
    ";
}

public class QMap
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public float tilesX;
    [UsedImplicitly] public float tilesY;
    [UsedImplicitly] public List<QTileDescriptor> tileDescriptors;
    [UsedImplicitly] public List<QEntityTraversalCompatibility> entityTraversalCompatibilities;

    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            tilesX: {tilesX}
            tilesY: {tilesY}
            tileDescriptors: [{string.Join(",", tileDescriptors)}]
            entityTraversalCompatibilities: [{string.Join(",", entityTraversalCompatibilities)}]
        }}";
    }
}