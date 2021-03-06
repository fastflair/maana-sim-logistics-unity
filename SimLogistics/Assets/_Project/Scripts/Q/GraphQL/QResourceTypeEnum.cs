using JetBrains.Annotations;

public class QResourceTypeEnum
{
    [UsedImplicitly] public string id;
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
        }}";
    }
}