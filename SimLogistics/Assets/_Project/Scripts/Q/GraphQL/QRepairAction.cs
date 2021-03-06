using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QRepairActionFragment
{
    public const string data = @"
        fragment repairActionData on RepairAction {
          id
          sim
          vehicle
          hub
        }";
}

public class QRepairAction
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string sim;
    [UsedImplicitly] public string vehicle;
    [UsedImplicitly] public string hub;
    
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            sim: ""{sim}""
            vehicle: ""{vehicle}""
            hub: ""{hub}""
        }}";
    }
}
