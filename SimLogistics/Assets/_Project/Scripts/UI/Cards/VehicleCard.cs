using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class VehicleCard : Card
{
    [SerializeField] private CardHost host;
    [SerializeField] private Sprite planeSprite;
    [SerializeField] private Sprite shipSprite;
    [SerializeField] private Sprite truckSprite;
    
    public void Populate(QVehicle qVehicle)
    {
        host.SetEntityId(qVehicle.id);
        host.SetThumbnail(ResolveThumbnail(qVehicle.type.id));

        // Properties
        // AddPropertyToGroup(propertyGroup1, "Speed", qVehicle.speed.ToString(CultureInfo.CurrentCulture));
        // AddPropertyToGroup(propertyGroup1, "Max Distance", qVehicle.maxDistance.ToString(CultureInfo.CurrentCulture));
        // AddPropertyToGroup(propertyGroup1, "Efficiency", qVehicle.efficiency.ToString(CultureInfo.CurrentCulture));
        // AddPropertyToGroup(propertyGroup1, "Durability", qVehicle.durability.ToString(CultureInfo.CurrentCulture));
        // AddPropertyToGroup(propertyGroup1, "Service Interval", qVehicle.serviceInterval.ToString(CultureInfo.CurrentCulture));
        // AddPropertyToGroup(propertyGroup1, "Last Service Step", $"{qVehicle.lastServiceStep.ToString(CultureInfo.CurrentCulture)} ({qVehicle.steps - qVehicle.lastServiceStep})");
        // AddPropertyToGroup(propertyGroup1, "Cargo Mode AND?", qVehicle.cargoModeAND.ToString(CultureInfo.CurrentCulture));

        // Cargo
        // foreach (var resource in qVehicle.cargo)
        //     AddResourceToGroup(propertyGroup2, resource);
    }
    
    private Sprite ResolveThumbnail(string id)
    {
        return id switch
        {
            "Plane" => planeSprite,
            "Ship" => shipSprite,
            "Truck" => truckSprite,
            _ => null
        };
    }

}