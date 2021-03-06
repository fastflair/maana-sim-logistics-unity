using JetBrains.Annotations;

public class QVehicleTypeEnum
{
    [UsedImplicitly] public string id;
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
        }}";
    }
}