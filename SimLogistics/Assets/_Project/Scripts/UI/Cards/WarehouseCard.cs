using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class WarehouseCard : Card
{
    [SerializeField] private CardHost host;
    [SerializeField] private Sprite warehouseSprite;
    
    public void Populate(QProducer qProducer)
    {
        host.SetEntityId(qProducer.id);
        host.SetThumbnail(warehouseSprite);

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
}