using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QCityFragment
{
    public const string data = @"
        fragment cityData on City {
          id
          sim
          steps
          x
          y
          yRot
          population
          populationGrowthRate
          populationDeclineRate
          demand { ...resourceData }
        }";

    public static string withIncludes => @$"
      {QResourceFragment.data}
      {data}
    ";
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
            populationGrowthRate: {populationGrowthRate}
            populationDeclineRate: {populationDeclineRate}
            demand: [{string.Join(",", demand)}]
        }}";
    }
}