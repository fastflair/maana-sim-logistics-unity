using JetBrains.Annotations;

public class QId
{
    [UsedImplicitly] public string id;
    
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
        }}";
    }
}