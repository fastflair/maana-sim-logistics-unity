using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QTileFragment
{
    public const string data = @"
        fragment tileData on TileOutput {
          id
          map
          type { id }
          x
          y
        }";
}

public class QTileTypeEnum
{
    public string id;
}

public class QTile
{
    public string id;
    public string map;
    public QTileTypeEnum type;
    public float x;
    public float y;
}