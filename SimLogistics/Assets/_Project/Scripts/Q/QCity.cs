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
    [UsedImplicitly] public float population;
    [UsedImplicitly] public float populationGrowthRate;
    [UsedImplicitly] public float populationDeclineRate;
    [UsedImplicitly] public List<QResource> demand;
    
    public override string ToString()
    {
        return @$"{{
            {base.ToString()}
            population: {population}
            population: {populationGrowthRate}
            population: {populationDeclineRate}
            demand: [{string.Join(",", demand)}]
        }}";
    }
}