using JetBrains.Annotations;

public class QProducerTypeEnum
{
    [UsedImplicitly] public string id;
    
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
        }}";
    }
}