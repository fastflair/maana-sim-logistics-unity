using System;
using System.Globalization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class HubCard : Card
{
    [SerializeField] private CardHost host;
    [SerializeField] private Sprite airportSprite;
    [SerializeField] private Sprite portSprite;
    [SerializeField] private Sprite truckDepotSprite;

    [SerializeField] protected Transform suppliesItemList;
    [SerializeField] protected Transform propertyItemList;
    [SerializeField] protected Transform noteItemList;

    public void Populate(QHub hub)
    {
        // print($"Populate hub: {hub}");
        
        ClearLists();
        
        host.SetThumbnail(ResolveThumbnail(hub.type.id));
        host.SetEntityId(hub.id);

        foreach (var resource in hub.supplies)
            AddResourceToList(suppliesItemList, resource);

        AddPropertyToList(propertyItemList, "Repair Surcharge", hub.repairSurcharge.ToString(CultureInfo.CurrentCulture));
    }

    private void ClearLists()
    {
        ClearList(suppliesItemList);
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