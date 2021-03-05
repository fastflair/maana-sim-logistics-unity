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

    protected override string QueryName => "selectProducer";
    protected override string Query => @$"
          {QResourceFragment.data}
          {QProducerFragment.data}
          query {{
            {QueryName}(sim: ""{simName.Value}"") {{
              ...producerData
            }}
          }}
        ";
    protected override Entity EntityPrefab(QProducer qProducer)
    {
        return qProducer.type.id switch
        {
            "CoalMine" => coalMinePrefab,
            "Cotton" => cottonPrefab,
            "Factory" => factoryPrefab,
            "Farm" => farmPrefab,
            "Forest" => forestPrefab,
            "LumberMill" => lumberMillPrefab,
            "PowerPlant" => powerPlantPrefab,
            "TextileMill" => textileMillPrefab,
            "OilRefinery" => oilRefineryPrefab,
            "OilWell" => oilWellPrefab,
            "Warehouse" => warehousePrefab,
            _ => null
        };
    }
}
