using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleManager : EntityManager<QVehicle>
{
    [SerializeField] private Entity truckPrefab;
    [SerializeField] private Entity planePrefab;
    [SerializeField] private Entity shipPrefab;
    
    public override IEnumerable<QVehicle> QEntities => simulationManager.CurrentState.vehicles;

    public override Entity EntityPrefab(QVehicle qVehicle)
    {
        return qVehicle.type.id switch
        {
            "Plane" => planePrefab,
            "Ship" => shipPrefab,
            "Truck" => truckPrefab,
            _ => null
        };
    }

    private static bool IsMoving(string status)
    {
        switch (status)
        {
            case "InTransit":
            case "Arrived":
            case "Repair":
                return true;
            default:
                return false;
        }
    }

    public void OnSimulationUpdate()
    {
        foreach (var vehicle in simulationManager.CurrentState.vehicles)
        {
            var transitOrder = vehicle.transitOrder;
            var status = transitOrder.status.id;
            if (!IsMoving(status)) continue;
            
            var vehicleEntity = EntityById(vehicle.id);
            var visited = new Queue<QWaypoint>(transitOrder.visited);
            VisitWaypoints(vehicle, vehicleEntity.gameObject, visited);
        }
    }
}