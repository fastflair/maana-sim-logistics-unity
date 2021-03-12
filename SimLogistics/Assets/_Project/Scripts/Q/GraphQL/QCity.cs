using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QCityFragment
{
    public const string Kind = "City";
    public static readonly string OutputKind = $"{Kind}Output";

    private const string fragmentTemplate = @"
        fragment cityData on {0} {{
          id
          sim
          steps
          x
          y
          yRot
          population
          populationGrowthRate
          populationDeclineRate
          demand {{ ...resourceData }}
        }}";

    public static string Data => string.Format(fragmentTemplate, Kind);
    public static readonly string OutputData = string.Format(fragmentTemplate, OutputKind);

    public static string DataWithIncludes => @$"
      {QResourceFragment.Data}
      {Data}
    ";
    
    public static string OutputDataWithIncludes => @$"
      {QResourceFragment.OutputData}
      {OutputData}
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