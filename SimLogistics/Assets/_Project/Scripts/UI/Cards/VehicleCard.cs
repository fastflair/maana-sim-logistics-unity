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
    
    [SerializeField] protected Transform cargoItemList;
    [SerializeField] protected Transform propertyItemList;
    [SerializeField] protected Transform ordersItemList;
    [SerializeField] protected Transform noteItemList;

    public void Populate(QVehicle qVehicle)
    {
        ClearLists();
        
        host.SetEntityId(qVehicle.id);
        host.SetThumbnail(ResolveThumbnail(qVehicle.type.id));

        foreach (var resource in qVehicle.cargo)
            AddResourceToList(cargoItemList, resource);

        AddPropertyToList(ordersItemList, "Status", qVehicle.transitOrder.status.id);
        AddPropertyToList(ordersItemList, "DestX", qVehicle.transitOrder.destX.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(ordersItemList, "DestY", qVehicle.transitOrder.destY.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(ordersItemList, "Steps", qVehicle.transitOrder.steps.ToString(CultureInfo.CurrentCulture));

        AddPropertyToList(propertyItemList, "X", qVehicle.x.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Y", qVehicle.y.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Speed", qVehicle.speed.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Max Distance", qVehicle.maxDistance.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Efficiency", qVehicle.efficiency.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Durability", qVehicle.durability.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Service Interval", qVehicle.serviceInterval.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Last Service Step", $"{qVehicle.lastServiceStep.ToString(CultureInfo.CurrentCulture)} ({qVehicle.steps - qVehicle.lastServiceStep})");
        AddPropertyToList(propertyItemList, "Cargo Mode AND?", qVehicle.cargoModeAND.ToString(CultureInfo.CurrentCulture));
    }
    
    private void ClearLists()
    {
        ClearList(cargoItemList);
        ClearList(ordersItemList);
        ClearList(propertyItemList);
        ClearList(noteItemList);
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