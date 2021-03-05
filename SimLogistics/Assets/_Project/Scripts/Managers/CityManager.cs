using System.Collections.Generic;
using UnityEngine;

public class CityManager : EntityManager<QCity>
{
    [SerializeField] private Entity cityPrefab;
    
    protected override IEnumerable<QCity> QEntities => simulationManager.CurrentState.cities;

    protected override Entity EntityPrefab(QCity qCity)
    {
        return cityPrefab;
    }
}