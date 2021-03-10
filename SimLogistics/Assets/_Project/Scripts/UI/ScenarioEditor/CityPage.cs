using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityPage : EditorPage<QCity>
{
    [SerializeField] protected InputFieldItem cityPopulation;
    [SerializeField] protected InputFieldItem cityPopulationGrowthRate;
    [SerializeField] protected InputFieldItem cityPopulationDeclineRate;

    protected override void PopulateEntityInputFields(QCity city)
    {
        cityPopulation.Value = ConvertFloat(city?.population ?? 0f);
        cityPopulationGrowthRate.Value = ConvertFloat(city?.populationGrowthRate ?? 0f);
        cityPopulationDeclineRate.Value =
            ConvertFloat(city?.populationDeclineRate ?? 0f);

        if (city == null) return;
        var addResourceItems = AddResourceItems(city.demand).ToList();
    }

    protected override void AddNewEntity(string id)
    {
        var city = new QCity
        {
            id = id,
            sim = scenarioEditor.Scenario,
            demand = new List<QResource>()
        };
        PopulateAllEntityInputFields(city);
    }

    protected override void SaveEntity(QCity city, Action<bool> callback)
    {
        city.population = ConvertFloat(cityPopulation.Value);
        city.populationGrowthRate = ConvertFloat(cityPopulationGrowthRate.Value);
        city.populationDeclineRate = ConvertFloat(cityPopulationDeclineRate.Value);
        print($"Saving city: {city}");

        scenarioEditor.simulationManager.SaveCity(city, id =>
        {
            callback.Invoke(id != null);
        });
    }

    protected override void DeleteEntity(QCity city, Action<bool> callback)
    {
        scenarioEditor.simulationManager.DeleteCity(city.id, id =>
        {
            callback.Invoke(id != null);
        });
    }
}