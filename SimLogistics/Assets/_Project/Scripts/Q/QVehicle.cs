using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QVehicleFragment
{
    public const string data = @"
        fragment vehicleData on VehicleOutput {
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
          durability
          serviceInterval
          lastServiceStep
          cargoModeAND
          transitOrder { ...transitOrderData }
        }";
}

public class QVehicleTypeEnum
{
    public string id;
}

public class QVehicle : QEntity
{
    public string hub;
    public QVehicleTypeEnum type;
    public List<QResource> cargo;
    public float speed;
    public float maxDistance;
    public float efficiency;
    public float durability;
    public int serviceInterval;    
    public int lastServiceStep;    
    public bool cargoModeAND;
    public QTransitOrder transitOrder;
}