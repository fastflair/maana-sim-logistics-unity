using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QTileFragment
{
    public const string data = @"
        fragment tileData on Tile {
          id
          map
          type { id }
          x
          y
          yRot
          traversalType { id }
          dockInfo { ...dockInfoData }
        }";
    
    public static readonly string withIncludes = @$"
      {QDockInfoFragment.data}
      {data}
    ";
}

public class QTile
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string map;
    [UsedImplicitly] public QTileTypeEnum type;
    [UsedImplicitly] public float x;
    [UsedImplicitly] public float y;
    [UsedImplicitly] public float yRot;
    [UsedImplicitly] public QTraversalTypeEnum traversalType;
    [UsedImplicitly] public List<QDockInfo> dockInfo;
    
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            map: ""{map}""
            type: {type}
            x: {x}
            y: {y}
            yRot: {yRot}
            traversalType: {traversalType}
            dockInfo: [{string.Join(",", dockInfo)}]
        }}";
    }
}