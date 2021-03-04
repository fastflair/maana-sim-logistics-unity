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
          type { id }
          quantity
          price
          status { id }
        }";
}

public class QResourceTransferTypeEnum
{
    public string id;
}

public class QResourceTransferStatusEnum
{
    public string id;
}

public class QResourceTransfer
{
    public string id;
    public string sim;
    public string vehicle;
    public string counterparty;
    public QResourceTransferTypeEnum type;
    public float quantity;
    public float price;
    public QResourceTransferStatusEnum status;
}