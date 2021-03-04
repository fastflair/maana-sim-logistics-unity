using System;
using System.Collections;
using System.Collections.Generic;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class VehicleManager : EntityManager<QVehicle, Vehicle>
{
    [SerializeField] private Vehicle truckPrefab;
    [SerializeField] private Vehicle planePrefab;
    [SerializeField] private Vehicle shipPrefab;

    protected override string QueryName => "selectVehicle";
    protected override string Query => @$"
          {QTransitOrderFragment.data}
          {QResourceFragment.data}
          {QVehicleFragment.data}
          query {{
            {QueryName}(sim: ""{simName.Value}"") {{
              ...vehicleData
            }}
          }}
        ";
    protected override Vehicle EntityPrefab(QVehicle qVehicle)
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