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
          destX
          destY      
        }";
}

public class QTransitOrder
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string sim;
    [UsedImplicitly] public string vehicle;
    [UsedImplicitly] public int steps;
    [UsedImplicitly] public QTransitStatusEnum status;
    [UsedImplicitly] public float destX;
    [UsedImplicitly] public float destY;

    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            sim: ""{sim}""
            vehicle: ""{vehicle}""
            steps: {steps}
            status: {status}
            destX: {destX}
            destY: {destY}
        }}";
    }
}