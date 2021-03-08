using System;
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

    public void Populate(QHub qHub)
    {
        ClearLists();
        
        host.SetThumbnail(ResolveThumbnail(qHub.type.id));
        host.SetEntityId(qHub.id);

        foreach (var resource in qHub.supplies)
            AddResourceToList(suppliesItemList, resource);
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