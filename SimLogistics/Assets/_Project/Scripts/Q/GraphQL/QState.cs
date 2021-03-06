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
          transitOrders { ...transitOrderData }
          mapAndTiles { ...mapAndTilesData }
        }";

    public static readonly string withIncludes = @$"
      {QMapAndTilesFragment.withIncludes}
      {QTransitOrderFragment.withIncludes}
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
    [UsedImplicitly] public List<QProducer> producers;
    [UsedImplicitly] public List<QHub> hubs;
    [UsedImplicitly] public List<QVehicle> vehicles;
    [UsedImplicitly] public List<QResourceTransfer> transfers;
    [UsedImplicitly] public List<QTransitOrder> transitOrders;
    [UsedImplicitly] public QMapAndTiles mapAndTiles;

    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            sim: {sim}
            cities: [{string.Join(",", cities)}]
            producers: [{string.Join(",", producers)}]
            hubs: [{string.Join(",", hubs)}]
            vehicles: [{string.Join(",", vehicles)}]
            transfers: [{string.Join(",", transfers)}]
            transitOrders: [{string.Join(",", transitOrders)}]
            mapAndTiles: {mapAndTiles}
        }}";
    }
}