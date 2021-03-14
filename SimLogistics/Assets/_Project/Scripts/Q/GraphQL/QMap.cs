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
        }";
}

public class QMap
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public float tilesX;
    [UsedImplicitly] public float tilesY;

    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            tilesX: {tilesX}
            tilesY: {tilesY}
        }}";
    }
}