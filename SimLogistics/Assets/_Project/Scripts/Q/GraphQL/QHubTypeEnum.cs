using JetBrains.Annotations;

public class QHubTypeEnum
{
    [UsedImplicitly] public string id;
    
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
        }}";
    }
}