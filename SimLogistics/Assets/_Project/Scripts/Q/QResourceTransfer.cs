using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QResourceTransferFragment
{
    public const string data = @"
        fragment resourceTransferData on ResourceTransferOutput {
          id
          sim
          vehicle
          counterparty
          transferType { id }
          resourceType { id }
          quantity
          price
          status { id }
        }";
}

public class QResourceTransferTypeEnum
{
    [UsedImplicitly] public string id;
}

public class QResourceTransferStatusEnum
{
    [UsedImplicitly] public string id;
}

public class QResourceTransfer
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string sim;
    [UsedImplicitly] public string vehicle;
    [UsedImplicitly] public string counterparty;
    [UsedImplicitly] public QResourceTransferTypeEnum transferType;
    [UsedImplicitly] public QResourceTypeEnum resourceType;
    [UsedImplicitly] public float quantity;
    [UsedImplicitly] public float price;
    [UsedImplicitly] public QResourceTransferStatusEnum status;
}