using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QResourceFragment
{
    public const string data = @"
        fragment resourceData on ResourceOutput {
          id
          sim
          type { id }
          capacity
          quantity
          basePricePerUnit
          adjustedPricePerUnit
          scarcitySurchargeFactor
          replenishRate
          consumptionRate
        }";
}

public class QResourceTypeEnum
{
    [UsedImplicitly] public string id;
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
}