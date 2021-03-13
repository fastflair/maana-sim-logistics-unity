using System.Collections.Generic;
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
}