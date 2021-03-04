using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QHubFragment
{
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
        }";
}

public class QHubTypeEnum
{
    public string id;
}

public class QHub : QEntity
{
    public QHubTypeEnum type;
    public List<QResource> supplies;
    public QVehicleTypeEnum vehicleType;
}