using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QDockInfoFragment
{
    public const string data = @"
        fragment dockInfoData on DockInfo {
          id
          vehicleType
          xOffset
          yOffset
          yRot
        }";
}

public class QDockInfo
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