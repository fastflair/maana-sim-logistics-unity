using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QHubFragment
{
    public static readonly string withIncludes = @$"
      {QResourceFragment.OutputData}
      {data}
    ";
    
    public const string data = @"
        fragment hubData on HubOutput {
          id
          sim
          steps
          x
          y
          type { id }
          supplies { ...resourceData }
          vehicleType { id }
          repairSurcharge
        }";
}

public class QHub : QEntity
{
    [UsedImplicitly] public QHubTypeEnum type;
    [UsedImplicitly] public List<QResource> supplies;
    [UsedImplicitly] public QVehicleTypeEnum vehicleType;
    [UsedImplicitly] public float repairSurcharge;
    
    public override string ToString()
    {
        return @$"{{
            {base.ToString()}
            type: {type}
            supplies: [{string.Join(",", supplies)}]
            vehicleType: {vehicleType}
            repairSurcharge: {repairSurcharge}
        }}";
    }
}