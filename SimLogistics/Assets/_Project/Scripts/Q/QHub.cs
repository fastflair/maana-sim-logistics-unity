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
    [UsedImplicitly] public string id;
}

public class QHub : QEntity
{
    [UsedImplicitly] public QHubTypeEnum type;
    [UsedImplicitly] public List<QResource> supplies;
    [UsedImplicitly] public QVehicleTypeEnum vehicleType;
}