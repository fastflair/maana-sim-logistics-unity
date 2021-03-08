using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ProducerCard : Card
{
    [SerializeField] private CardHost host;
    [SerializeField] private Sprite coalMineSprite;
    [SerializeField] private Sprite cottonSprite;
    [SerializeField] private Sprite farmSprite;
    [SerializeField] private Sprite factorySprite;
    [SerializeField] private Sprite forestSprite;
    [SerializeField] private Sprite lumberMillSprite;
    [SerializeField] private Sprite oilRefinerySprite;
    [SerializeField] private Sprite oilWellSprite;
    [SerializeField] private Sprite powerPlantSprite;
    [SerializeField] private Sprite textileMillSprite;
    [SerializeField] private Sprite warehouseSprite;
    
    public void Populate(QProducer qProducer)
    {
        host.SetEntityId(qProducer.id);
        host.SetThumbnail(ResolveThumbnail(qProducer.type.id));

        // AddPropertyToGroup(propertyGroup1, "Stoppage Surcharge Factor", qProducer.stoppageSurchargeFactor.ToString(CultureInfo.CurrentCulture));
        //
        // foreach (var resource in qProducer.material)
        //     AddResourceToGroup(propertyGroup2, resource);
        //
        // foreach (var resource in qProducer.products)
        //     AddResourceToGroup(propertyGroup3, resource);
    }
    
    private Sprite ResolveThumbnail(string id)
    {
        return id switch
        {
            "CoalMine" => coalMineSprite,
            "Cotton" => cottonSprite,
            "Farm" => farmSprite,
            "Factory" => factorySprite,
            "Forest" => forestSprite,
            "LumberMill" => lumberMillSprite,
            "OilRefinery" => oilRefinerySprite,
            "OilWell" => oilWellSprite,
            "PowerPlant" => powerPlantSprite,
            "TextileMill" => textileMillSprite,
            "Warehouse" => warehouseSprite,
            _ => cottonSprite
        };
    }
}