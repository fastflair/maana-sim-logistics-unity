using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QStateFragment
{
    public const string data = @"
        fragment stateData on State {
          id
          simulation { ...simulationData }
          cities { ...cityData }
          producers { ...producerData }
          hubs { ...hubData }
          vehicles { ...vehicleData }
          transfers { ...resourceTransferData }
        }";
}

public class QState
{
    public string id;
    public QSimulation simulation;
    public List<QCity> cities;
    public List<QProducer> producers;
    public List<QHub> hubs;
    public List<QVehicle> vehicles;
    public List<QResourceTransfer> transfers;
}
