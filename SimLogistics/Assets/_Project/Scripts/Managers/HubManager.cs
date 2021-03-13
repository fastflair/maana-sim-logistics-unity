using System.Collections.Generic;
using UnityEngine;

public class HubManager : EntityManager<QHub>
{
    [SerializeField] private Entity portPrefab;
    [SerializeField] private Entity airportPrefab;
    [SerializeField] private Entity truckDepotPrefab;

    public override IEnumerable<QHub> QEntities => simulationManager.CurrentState.hubs;

    public override Entity EntityPrefab(QHub qHub)
    {
        return qHub.type.id switch
        {
            "Airport" => airportPrefab,
            "Port" => portPrefab,
            "TruckDepot" => truckDepotPrefab,
            _ => null
        };
    }
}