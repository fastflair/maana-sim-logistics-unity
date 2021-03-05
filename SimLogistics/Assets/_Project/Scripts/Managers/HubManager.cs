using UnityEngine;

public class HubManager : EntityManager<QHub>
{
  [SerializeField] private Entity portPrefab;
  [SerializeField] private Entity airportPrefab;
  [SerializeField] private Entity truckDepotPrefab;

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
    protected override Entity EntityPrefab(QHub qHub)
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
