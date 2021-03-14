using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QTileFragment
{
    public const string data = @"
        fragment tileData on Tile {
          id
          map
          type
          x
          y
          yRot
          docks { ...dockData }
        }";
    
    public static readonly string withIncludes = @$"
      {QDockFragment.data}
      {data}
    ";
}

public class QTile
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string map;
    [UsedImplicitly] public string type;
    [UsedImplicitly] public float x;
    [UsedImplicitly] public float y;
    [UsedImplicitly] public float yRot;
    [UsedImplicitly] public List<QDock> docks;
    
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            map: ""{map}""
            type: ""{type}""
            x: {x}
            y: {y}
            yRot: {yRot}
            docks: [{string.Join(",", docks)}]
        }}";
    }
}