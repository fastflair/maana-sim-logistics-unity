using UnityEngine;

public class CityManager : EntityManager<QCity, City>
{
    [SerializeField] private City cityPrefab;

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
    protected override City EntityPrefab(QCity qCity)
    {
      return cityPrefab;
    }
}
