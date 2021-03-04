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
    public string id;
}

public class QResource
{
    public string id;
    public string sim;
    public QResourceTypeEnum type;
    public float capacity;
    public float quantity;
    public float basePricePerUnit;
    public float adjustedPricePerUnit;
    public float scarcitySurchargeFactor;
    public float replenishRate;
    public float consumptionRate;
}