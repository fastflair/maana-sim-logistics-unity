using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QMapAndTilesFragment
{
    public const string data = @"
        fragment mapAndTilesData on MapAndTiles {
          id
          map { ...mapData }
          tiles { ...tileData }
        }";
    
    public static readonly string withIncludes = @$"
      {QMapFragment.withIncludes}
      {QTileFragment.withIncludes}
      {data}
    ";}

public class QMapAndTiles
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public QMap map;
    [UsedImplicitly] public List<QTile> tiles;

    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            map: {map}
            tiles: [{string.Join(",", tiles)}]
        }}";
    }
}