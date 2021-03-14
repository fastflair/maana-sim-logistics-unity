using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QTransitActionFragment
{
    public const string data = @"
        fragment transitActionData on TransitAction {
          id
          sim
          vehicle
          waypoints { ...waypointData }
        }";
    
    public static readonly string withIncludes = @$"
      {QWaypointFragment.data}
      {data}
    ";
}

public class QTransitAction
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string sim;
    [UsedImplicitly] public string vehicle;
    [UsedImplicitly] public List<QWaypoint> waypoints;
    
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            sim: ""{sim}""
            vehicle: ""{vehicle}""
            waypoints: [{string.Join(",", waypoints)}]
        }}";
    }
}
