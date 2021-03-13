using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class VehicleCard : Card
{
    [SerializeField] private Sprite planeSprite;
    [SerializeField] private Sprite shipSprite;
    [SerializeField] private Sprite truckSprite;

    [SerializeField] protected TransitOrderItem transitOrderItemPrefab;

    [SerializeField] protected Transform cargoItemList;
    [SerializeField] protected Transform propertyItemList;
    [SerializeField] protected Transform noteItemList;

    public void Populate(QVehicle vehicle)
    {
        ClearLists();
        
        host.SetEntityId(vehicle.id);
        host.SetThumbnail(ResolveThumbnail(vehicle.type.id));

        foreach (var resource in vehicle.cargo)
            AddResourceToList(cargoItemList, resource);

        var transitOrderItem = Instantiate(transitOrderItemPrefab, ordersItemList, false);
        transitOrderItem.Populate(vehicle.transitOrder);

        var col = host.simulationManager.CurrentState.transfers.FindAll(x => x.vehicle == vehicle.id);
        col.Sort(SimulationManager.TransferComparer);
        foreach (var transferOrder in col)
        {
            var transferOrderItem = Instantiate(transferOrderItemPrefab, ordersItemList, false);
            transferOrderItem.Populate(transferOrder);
        }
        
        AddPropertyToList(propertyItemList, "X", vehicle.x.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Y", vehicle.y.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Fuel",
            vehicle.fuel == null ? "!!! NULL !!!" : SimulationManager.FormatResourceVolumeDisplay(vehicle.fuel));

        AddPropertyToList(propertyItemList, "Speed", vehicle.speed.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Max Distance", vehicle.maxDistance.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Efficiency", vehicle.efficiency.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Durability", vehicle.durability.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Service Interval", vehicle.serviceInterval.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Last Service Step", $"{vehicle.lastServiceStep.ToString(CultureInfo.CurrentCulture)} ({vehicle.steps - vehicle.lastServiceStep})");
        AddPropertyToList(propertyItemList, "Cargo Mode AND?", vehicle.cargoModeAND.ToString(CultureInfo.CurrentCulture));
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