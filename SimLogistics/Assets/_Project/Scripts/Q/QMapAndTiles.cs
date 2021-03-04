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
}

public class QMapAndTiles
{
    public string id;
    public QMap map;
    public List<QTile> tiles;
}