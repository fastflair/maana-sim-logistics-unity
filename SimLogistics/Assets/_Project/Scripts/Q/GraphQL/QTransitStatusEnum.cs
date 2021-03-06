using JetBrains.Annotations;

public class QTransitStatusEnum
{
    [UsedImplicitly] public string id;
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
        }}";
    }
}