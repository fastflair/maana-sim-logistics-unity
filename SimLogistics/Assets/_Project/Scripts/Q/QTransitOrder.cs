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
    public string id;
}

public class QTransitOrder
{
    public string id;
    public string sim;
    public string vehicle;
    public int steps;
    public QTransitStatusEnum status;
    public float destX;
    public float destY;
}