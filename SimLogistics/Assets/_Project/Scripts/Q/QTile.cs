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
    [UsedImplicitly] public string id;
}

public class QTile
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string map;
    [UsedImplicitly] public QTileTypeEnum type;
    [UsedImplicitly] public float x;
    [UsedImplicitly] public float y;
}