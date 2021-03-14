using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QWaypointFragment
{
    public const string data = @"
        fragment waypointData on Waypoint {
          id
          order
          x
          y
        }";
}

public class QWaypoint
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public int order;
    [UsedImplicitly] public float x;
    [UsedImplicitly] public float y;
    
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            order: {order}
            x: {x}
            y: {y}
        }}";
    }
}