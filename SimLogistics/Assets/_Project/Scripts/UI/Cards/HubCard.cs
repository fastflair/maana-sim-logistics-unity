using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class HubCard : Card
{
    [SerializeField] private Sprite airportSprite;
    [SerializeField] private Sprite portSprite;
    [SerializeField] private Sprite truckDepotSprite;

    [SerializeField] protected Transform suppliesItemList;
    [SerializeField] protected Transform vehiclesItemList;
    [SerializeField] protected Transform propertyItemList;
    [SerializeField] protected Transform noteItemList;

    [SerializeField] private VehicleInfoItem vehicleInfoItemPrefab;

    public void Populate(QHub hub)
    {
        // print($"Populate hub: {hub}");

        ClearLists();

        host.SetThumbnail(ResolveThumbnail(hub.type.id));
        host.SetEntityId(hub.id);

        foreach (var resource in hub.supplies)
            AddResourceToList(suppliesItemList, resource);

        foreach (var vehicle in host.simulationManager.CurrentState.vehicles.FindAll(x => x.hub == hub.id))
        {
            var vehicleInfoItem = Instantiate(vehicleInfoItemPrefab, vehiclesItemList, false);
            vehicleInfoItem.Populate(vehicle);
        }

        AddPropertyToList(propertyItemList, "Repair Surcharge",
            hub.repairSurcharge.ToString(CultureInfo.CurrentCulture));
    }

    private void ClearLists()
    {
        ClearList(suppliesItemList);
        ClearList(vehiclesItemList);
        ClearList(propertyItemList);
        ClearList(noteItemList);
    }

    private Sprite ResolveThumbnail(string id)
    {
        return id switch
        {
            "Airport" => airportSprite,
            "Port" => portSprite,
            "TruckDepot" => truckDepotSprite,
            _ => null
        };
    }
}