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

    public void Populate(QHub qHub)
    {
        host.SetThumbnail(ResolveThumbnail(qHub.type.id));
        host.SetEntityId(qHub.id);

        // AddPropertyToGroup(propertyGroup1, "(none)", "");
        //
        // foreach (var resource in qHub.supplies)
        //     AddResourceToGroup(propertyGroup2, resource);
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