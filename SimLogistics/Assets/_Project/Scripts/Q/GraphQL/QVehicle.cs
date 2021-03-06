using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QVehicleFragment
{
    public static readonly string withIncludes = @$"
      {QTransitOrderFragment.data}
      {QResourceFragment.data}
      {data}
    ";
    
    public const string data = @"
        fragment vehicleData on Vehicle {
          id
          sim
          steps
          x
          y
          hub
          type { id }
          cargo { ...resourceData }
          speed
          maxDistance
          efficiency
          reliability
          durability
          serviceInterval
          lastServiceStep
          cargoModeAND
          transitOrder { ...transitOrderData }
        }";
}

public class QVehicle : QEntity
{
    [UsedImplicitly] public string hub;
    [UsedImplicitly] public QVehicleTypeEnum type;
    [UsedImplicitly] public List<QResource> cargo;
    [UsedImplicitly] public float speed;
    [UsedImplicitly] public float maxDistance;
    [UsedImplicitly] public float efficiency;
    [UsedImplicitly] public float reliability;
    [UsedImplicitly] public float durability;
    [UsedImplicitly] public int serviceInterval;    
    [UsedImplicitly] public int lastServiceStep;    
    [UsedImplicitly] public bool cargoModeAND;
    [UsedImplicitly] public QTransitOrder transitOrder;
    
    public override string ToString()
    {
        return @$"{{
            {base.ToString()}
            hub: ""{hub}""
            type: {type}
            cargo: [{string.Join(",", cargo)}]
            speed: {speed}
            maxDistance: {maxDistance}
            efficiency: {efficiency}
            reliability: {reliability}
            durability: {durability}
            serviceInterval: {serviceInterval}
            lastServiceStep: {lastServiceStep}
            cargoModeAND: {cargoModeAND.ToString().ToLower()}
            transitOrder: {transitOrder}
        }}";
    }
}