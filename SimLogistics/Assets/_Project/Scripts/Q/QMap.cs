using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QMapFragment
{
    public const string data = @"
        fragment mapData on MapOutput {
          id
          tilesX
          tilesY
        }";
}

public class QMap
{
    public string id;
    public float tilesX;
    public float tilesY;
}