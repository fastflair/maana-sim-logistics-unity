using JetBrains.Annotations;

public class QResourceTransferTypeEnum
{
    [UsedImplicitly] public string id;
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
        }}";
    }
}