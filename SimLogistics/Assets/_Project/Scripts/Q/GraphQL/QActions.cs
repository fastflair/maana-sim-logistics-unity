using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

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
      {QWaypointFragment.data}
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

    public QActions()
    {
        transitActions = new List<QTransitAction>(); 
        repairActions = new List<QRepairAction>();
        transferActions = new List<QTransferAction>();
    }
    
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
