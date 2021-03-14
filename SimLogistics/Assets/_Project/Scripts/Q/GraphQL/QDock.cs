using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QDockFragment
{
    public const string data = @"
        fragment dockData on Dock {
          id
          vehicleType
          xOffset
          yOffset
          yRot
        }";
}

public class QDock
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string vehicleType;
    [UsedImplicitly] public float xOffset;
    [UsedImplicitly] public float yOffset;
    [UsedImplicitly] public float yRot;
    
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            vehicleType: ""{vehicleType}""
            xOffset: {xOffset}
            yOffset: {yOffset}
            yRot: {yRot}
        }}";
    }

}