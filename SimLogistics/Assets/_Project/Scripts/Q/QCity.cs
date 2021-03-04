using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QCityFragment
{
    public const string data = @"
        fragment cityData on CityOutput {
          id
          sim
          steps
          x
          y
          population
          populationGrowthRate
          populationDeclineRate
          demand { ...resourceData }
        }";
}

public class QCity : QEntity
{
    public float population;
    public float populationGrowthRate;
    public float populationDeclineRate;
    public List<QResource> demand;
}