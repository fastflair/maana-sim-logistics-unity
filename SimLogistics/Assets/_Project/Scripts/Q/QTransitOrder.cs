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

public class QTransitStatusEnum
{
    [UsedImplicitly] public string id;
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
}