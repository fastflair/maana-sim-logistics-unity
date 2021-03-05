using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class VehicleCard : Card
{
    [SerializeField] private Sprite planeSprite;
    [SerializeField] private Sprite shipSprite;
    [SerializeField] private Sprite truckSprite;
    
    public override void Populate(Card card, QEntity qEntity)
    {
        var qVehicle = qEntity as QVehicle;
        var vehicleCard = card as VehicleCard;

        if (qVehicle == null) throw new Exception("QEntity is not of the expected type.");
        if (vehicleCard == null) throw new Exception("Card is not of the expected type.");

        SetTitle(qVehicle.id);
        SetThumbnail(ResolveThumbnail(qVehicle.type.id));

        // Properties
        AddPropertyToGroup(propertyGroup1, "Speed", qVehicle.speed.ToString(CultureInfo.CurrentCulture));
        AddPropertyToGroup(propertyGroup1, "Max Distance", qVehicle.maxDistance.ToString(CultureInfo.CurrentCulture));
        AddPropertyToGroup(propertyGroup1, "Efficiency", qVehicle.efficiency.ToString(CultureInfo.CurrentCulture));
        AddPropertyToGroup(propertyGroup1, "Durability", qVehicle.durability.ToString(CultureInfo.CurrentCulture));
        AddPropertyToGroup(propertyGroup1, "Service Interval", qVehicle.serviceInterval.ToString(CultureInfo.CurrentCulture));
        AddPropertyToGroup(propertyGroup1, "Last Service Step", $"{qVehicle.lastServiceStep.ToString(CultureInfo.CurrentCulture)} ({qVehicle.steps - qVehicle.lastServiceStep})");
        AddPropertyToGroup(propertyGroup1, "Cargo Mode AND?", qVehicle.cargoModeAND.ToString(CultureInfo.CurrentCulture));

        // Cargo
        foreach (var resource in qVehicle.cargo)
            AddResourceToGroup(propertyGroup2, resource);
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