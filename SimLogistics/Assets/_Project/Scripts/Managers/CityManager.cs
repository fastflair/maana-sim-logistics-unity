using UnityEngine;

public class CityManager : EntityManager<QCity>
{
    [SerializeField] private Entity cityPrefab;

    protected override string QueryName => "selectCity";
    protected override string Query => @$"
          {QResourceFragment.data}
          {QCityFragment.data}
          query {{
            {QueryName}(sim: ""{simName.Value}"") {{
              ...cityData
            }}
          }}
        ";
    protected override Entity EntityPrefab(QCity qCity)
    {
      return cityPrefab;
    }
}
