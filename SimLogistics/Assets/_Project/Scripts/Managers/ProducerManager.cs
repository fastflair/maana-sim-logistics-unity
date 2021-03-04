using UnityEngine;

public class ProducerManager : EntityManager<QProducer, Producer>
{
    [SerializeField] private Producer coalMinePrefab;
    [SerializeField] private Producer cottonPrefab;
    [SerializeField] private Producer factoryPrefab;
    [SerializeField] private Producer farmPrefab;
    [SerializeField] private Producer forestPrefab;
    [SerializeField] private Producer lumberMillPrefab;
    [SerializeField] private Producer powerPlantPrefab;
    [SerializeField] private Producer textileMillPrefab;
    [SerializeField] private Producer oilRefineryPrefab;
    [SerializeField] private Producer oilWellPrefab;
    [SerializeField] private Producer warehousePrefab;

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
    protected override Producer EntityPrefab(QProducer qProducer)
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
