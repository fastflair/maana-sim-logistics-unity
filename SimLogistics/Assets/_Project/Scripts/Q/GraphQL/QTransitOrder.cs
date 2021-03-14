using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QTransitOrderFragment
{
    public const string data = @"
        fragment transitOrderData on TransitOrderOutput {
          id
          sim
          vehicle
          steps
          status { id }
          waypoints { ...waypointData }
          visited { ...waypointData }
        }";
    
    public static readonly string withIncludes = @$"
      {QWaypointFragment.data}
      {data}
    ";
}

public class QTransitOrder
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string sim;
    [UsedImplicitly] public string vehicle;
    [UsedImplicitly] public int steps;
    [UsedImplicitly] public QTransitStatusEnum status;
    [UsedImplicitly] public List<QWaypoint> waypoints;
    [UsedImplicitly] public List<QWaypoint> visited;

    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            sim: ""{sim}""
            vehicle: ""{vehicle}""
            steps: {steps}
            status: {status}
            waypoints: [{string.Join(",", waypoints)}]
            visited: [{string.Join(",", visited)}]
        }}";
    }
}