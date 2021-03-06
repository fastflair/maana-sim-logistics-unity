using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QActionsFragment
{
    public const string data = @"
        fragment actionsData on Actions {
          id
          transitActions { ...transitActionData }
          repairActions { ...repairActionData }
          transferActions { ...transferActionData }
        }";
    
    public static readonly string withIncludes = @$"
      {QTransitActionFragment.data}
      {QRepairActionFragment.data}
      {QTransferActionFragment.data}
      {data}
    ";
}

public class QActions
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public List<QTransitAction> transitActions;
    [UsedImplicitly] public List<QRepairAction> repairActions;
    [UsedImplicitly] public List<QTransferAction> transferActions;
  
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            transitActions: [{string.Join(",", transitActions)}]            
            repairActions: [{string.Join(",", repairActions)}]            
            transferActions: [{string.Join(",", transferActions)}]            
        }}";
    }
}
