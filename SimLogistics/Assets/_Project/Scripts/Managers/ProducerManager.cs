using System.Collections.Generic;
using UnityEngine;

public class ProducerManager : EntityManager<QProducer>
{
    [SerializeField] private Entity coalMinePrefab;
    [SerializeField] private Entity cottonPrefab;
    [SerializeField] private Entity factoryPrefab;
    [SerializeField] private Entity farmPrefab;
    [SerializeField] private Entity forestPrefab;
    [SerializeField] private Entity lumberMillPrefab;
    [SerializeField] private Entity powerPlantPrefab;
    [SerializeField] private Entity textileMillPrefab;
    [SerializeField] private Entity oilRefineryPrefab;
    [SerializeField] private Entity oilWellPrefab;
    [SerializeField] private Entity warehousePrefab;

    [SerializeField] private  Sprite coalMineSprite;
    [SerializeField] private Sprite cottonSprite;
    [SerializeField] private Sprite farmSprite;
    [SerializeField] private Sprite factorySprite;
    [SerializeField] private Sprite forestSprite;
    [SerializeField] private  Sprite lumberMillSprite;
    [SerializeField] private Sprite oilRefinerySprite;
    [SerializeField] private  Sprite oilWellSprite;
    [SerializeField] private Sprite powerPlantSprite;
    [SerializeField] private Sprite textileMillSprite;
    [SerializeField] private Sprite warehouseSprite;

    protected override IEnumerable<QProducer> QEntities => simulationManager.CurrentState.producers;

    public override Entity EntityPrefab(QProducer qProducer)
    {
        // print($"prefab for {qProducer.type.id}");
        return qProducer.type.id switch
        {
            "CoalMine" => coalMinePrefab,
            "Cotton" => cottonPrefab,
            "Factory" => factoryPrefab,
            "Farm" => farmPrefab,
            "Forest" => forestPrefab,
            "LumberMill" => lumberMillPrefab,
            "OilRefinery" => oilRefineryPrefab,
            "OilWell" => oilWellPrefab,
            "PowerPlant" => powerPlantPrefab,
            "TextileMill" => textileMillPrefab,
            "Warehouse" => warehousePrefab,
            _ => null
        };
    }
    
    public Sprite EntityThumbnail(string id)
    {
        return id switch
        {
            "CoalMine" => coalMineSprite,
            "Cotton" => cottonSprite,
            "Factory" => factorySprite,
            "Farm" => farmSprite,
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
    
    public Sprite ResourceThumbnail(string id)
    {
        return id switch
        {
            "Coal" => coalMineSprite,
            "Cotton" => cottonSprite,
            "Crude" => oilWellSprite,
            "Energy" => powerPlantSprite,
            "Food" => farmSprite,
            "Goods" => factorySprite,
            "Lumber" => lumberMillSprite,
            "Petrochemicals" => oilRefinerySprite,
            "Textiles" => textileMillSprite,
            "Trees" => forestSprite,
            _ => cottonSprite
        };
    }
    
}