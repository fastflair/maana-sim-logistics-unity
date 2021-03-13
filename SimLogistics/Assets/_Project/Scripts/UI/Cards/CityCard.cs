using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityCard : Card
{
    [SerializeField] private Sprite citySprite;

    [SerializeField] protected Transform resourceItemList;
    [SerializeField] protected Transform propertyItemList;
    [SerializeField] protected Transform noteItemList;

    public void Populate(QCity city)
    {
        // print($"Populate city: {city}");
        
        ClearLists();

        host.SetThumbnail(citySprite);
        host.SetEntityId(city.id);

        AddPropertyToList(propertyItemList, "Population", city.population.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Population Growth Rate",
            city.populationGrowthRate.ToString(CultureInfo.CurrentCulture));
        AddPropertyToList(propertyItemList, "Population Decline Rate",
            city.populationDeclineRate.ToString(CultureInfo.CurrentCulture));

        foreach (var resource in city.demand)
            AddResourceToList(resourceItemList, resource);
    }

    private void ClearLists()
    {
        ClearList(resourceItemList);
        ClearList(propertyItemList);
        ClearList(noteItemList);
    }
}