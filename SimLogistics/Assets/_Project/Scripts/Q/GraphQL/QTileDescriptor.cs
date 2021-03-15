using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QTileDescriptorFragment
{
    public const string data = @"
        fragment tileDescriptorData on TileDescriptor {
          id
          traversabilityGrid
        }";
}

public class QTileDescriptor
{
    [UsedImplicitly] public string id;
    [UsedImplicitly] public List<string> traversabilityGrid;

    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            traversabilityGrid: [{string.Join(",", traversabilityGrid.Select(x => $"\"{x}\""))}]
        }}";
    }
}