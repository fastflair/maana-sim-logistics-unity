using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityCard : Card
{
    [SerializeField] private CardHost host;
    [SerializeField] private Sprite citySprite;
    
    public  void Populate(QCity qCity)
    {
        host.SetThumbnail(citySprite);
        host.SetEntityId(qCity.id);

        // AddPropertyToGroup(propertyGroup1, "Population", qCity.population.ToString(CultureInfo.CurrentCulture));
        // AddPropertyToGroup(propertyGroup1, "Population Growth Rate",
        //     qCity.populationGrowthRate.ToString(CultureInfo.CurrentCulture));
        // AddPropertyToGroup(propertyGroup1, "Population Decline Rate",
        //     qCity.populationDeclineRate.ToString(CultureInfo.CurrentCulture));
        //
        // foreach (var resource in qCity.demand)
        //     AddResourceToGroup(propertyGroup2, resource);
    }
}