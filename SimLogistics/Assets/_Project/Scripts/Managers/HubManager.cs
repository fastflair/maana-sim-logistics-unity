using UnityEngine;

public class HubManager : EntityManager<QHub, Hub>
{
  [SerializeField] private Hub portPrefab;
  [SerializeField] private Hub airportPrefab;
  [SerializeField] private Hub truckDepotPrefab;

    protected override string QueryName => "selectHub";
    protected override string Query => @$"
          {QResourceFragment.data}
          {QHubFragment.data}
          query {{
            {QueryName}(sim: ""{simName.Value}"") {{
              ...hubData
            }}
          }}
        ";
    protected override Hub EntityPrefab(QHub qHub)
    {
      return qHub.type.id switch
      {
          "Airport" => airportPrefab,
          "Port" => portPrefab,
          "TruckDepot" => truckDepotPrefab,
          _ => null
      };
    }
}
