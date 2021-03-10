using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QResourceFragment
{
    public const string Kind = "Resource";
    public static readonly string OutputKind = $"{Kind}Output";

    public const string fragmentTemplate = @"
        fragment resourceData on {0} {{
          id
          sim
          type {{ id }}
          capacity
          quantity
          basePricePerUnit
          adjustedPricePerUnit
          scarcitySurchargeFactor
          replenishRate
          consumptionRate
        }}";
    
    public static string Data => string.Format(fragmentTemplate, Kind);
    public static readonly string OutputData = string.Format(fragmentTemplate, OutputKind);
}

public class QResource
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string sim;
    [UsedImplicitly] public QResourceTypeEnum type;
    [UsedImplicitly] public float capacity;
    [UsedImplicitly] public float quantity;
    [UsedImplicitly] public float basePricePerUnit;
    [UsedImplicitly] public float adjustedPricePerUnit;
    [UsedImplicitly] public float scarcitySurchargeFactor;
    [UsedImplicitly] public float replenishRate;
    [UsedImplicitly] public float consumptionRate;

    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            sim: ""{sim}""
            type: {type}
            capacity: {capacity}
            quantity: {quantity}
            basePricePerUnit: {basePricePerUnit}
            adjustedPricePerUnit: {adjustedPricePerUnit}
            scarcitySurchargeFactor: {scarcitySurchargeFactor}
            replenishRate: {replenishRate}
            consumptionRate: {consumptionRate}
        }}";
    }
}