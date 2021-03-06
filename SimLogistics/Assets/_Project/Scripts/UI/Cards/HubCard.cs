using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class HubCard : Card
{
    [SerializeField] private Sprite airportSprite;
    [SerializeField] private Sprite portSprite;
    [SerializeField] private Sprite truckDepotSprite;

    public override void Populate(Card card, QEntity qEntity)
    {
        var hubCard = card as HubCard;

        if (!(qEntity is QHub qHub)) throw new Exception("QEntity is not of the expected type.");
        if (hubCard == null) throw new Exception("Card is not of the expected type.");

        SetEntityId(qHub.id);
        SetThumbnail(ResolveThumbnail(qHub.type.id));

        AddPropertyToGroup(propertyGroup1, "(none)", "");
        
        foreach (var resource in qHub.supplies)
            AddResourceToGroup(propertyGroup2, resource);

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