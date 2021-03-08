using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityCard : Card
{
    [SerializeField] private CardHost host;
    [SerializeField] private Sprite citySprite;

    [SerializeField] protected Transform resourceItemList;
    [SerializeField] protected Transform propertyItemList;
    [SerializeField] protected Transform noteItemList;

    public  void Populate(QCity qCity)
    {
        ClearList(resourceItemList);
        ClearList(propertyItemList);
        ClearList(noteItemList);
        
        host.SetThumbnail(citySprite);
        host.SetEntityId(qCity.id);

        AddPropertyToList(propertyItemList, "Population", qCity.population.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Population Growth Rate",
            qCity.populationGrowthRate.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Population Decline Rate",
            qCity.populationDeclineRate.ToString(CultureInfo.CurrentCulture));
        
        foreach (var resource in qCity.demand)
            AddResourceToList(resourceItemList, resource);
    }
}