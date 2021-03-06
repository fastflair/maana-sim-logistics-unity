using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QTransitActionFragment
{
    public const string data = @"
        fragment transitActionData on TransitAction {
          id
          sim
          vehicle
          destX
          destY
        }";
}

public class QTransitAction
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string sim;
    [UsedImplicitly] public string vehicle;
    [UsedImplicitly] public float destX;
    [UsedImplicitly] public float destY;
    
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            sim: ""{sim}""
            vehicle: ""{vehicle}""
            destX: {destX}
            destY: {destY}
        }}";
    }
}
