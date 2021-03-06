using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable InconsistentNaming

// type Actions {
//   id: ID!
//   transitActions: [TransitAction!]!
//   repairActions: [RepairAction!]!
//   transferActions: [TransferAction!]!
// }
public static class QActionsFragment
{
    public static readonly string withIncludes = @$"
      {data}
    ";
    
    public const string data = @"
        fragment actionsData on Actions {
          id
        }";
}

public class QActions
{
    [UsedImplicitly] public string id;
  
    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
        }}";
    }
}
