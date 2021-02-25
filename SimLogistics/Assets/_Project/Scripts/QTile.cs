using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

[UsedImplicitly]
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