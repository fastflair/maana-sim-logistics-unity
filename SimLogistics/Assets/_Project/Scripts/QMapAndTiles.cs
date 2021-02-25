using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming
public class QMapAndTiles
{
    public string id;
    [UsedImplicitly] public QMap map;
    [UsedImplicitly] public List<QTile> tiles;
}