using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QStateFragment
{
    public const string data = @"
        fragment stateData on State {
          id
          sim { ...simulationData }
          cities { ...cityData }
          producers { ...producerData }
          hubs { ...hubData }
          vehicles { ...vehicleData }
          transfers { ...resourceTransferData }
        }";

    public static readonly string withIncludes = @$"
      {QTransitOrderFragment.data}
      {QResourceTransferFragment.data}
      {QResourceFragment.data}
      {QVehicleFragment.data}
      {QHubFragment.data}
      {QProducerFragment.data}
      {QCityFragment.data}
      {QSimulationFragment.data}
      {data}
    ";
}

public class QState
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public QSimulation sim;
    [UsedImplicitly] public List<QCity> cities;
    [UsedImplicitly] public List<QHub> hubs;
    [UsedImplicitly] public List<QProducer> producers;
    [UsedImplicitly] public List<QResourceTransfer> transfers;
    [UsedImplicitly] public List<QVehicle> vehicles;

    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            sim: {sim}
            cities: [{string.Join(",", cities)}]
            hubs: [{string.Join(",", hubs)}]
            producers: [{string.Join(",", producers)}]
            transfers: [{string.Join(",", transfers)}]
            vehicles: [{string.Join(",", vehicles)}]
        }}";
    }
}