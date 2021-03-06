using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QTransferActionFragment
{
    public const string data = @"
        fragment transferActionData on TransferAction {
          id
          sim
          vehicle
          counterparty
          resourceType { id }
          quantity
          transferType { id }
        }";
}

public class QTransferAction
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string sim;
    [UsedImplicitly] public string vehicle;
    [UsedImplicitly] public string counterparty;
    [UsedImplicitly] public QResourceTypeEnum resourceType;
    [UsedImplicitly] public float quantity;
    [UsedImplicitly] public QResourceTransferTypeEnum transferType;
    
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            sim: ""{sim}""
            vehicle: ""{vehicle}""
            counterparty: ""{counterparty}""
            resourceType: {resourceType}
            quantity: {quantity}
            transferType: {transferType}
        }}";
    }
}