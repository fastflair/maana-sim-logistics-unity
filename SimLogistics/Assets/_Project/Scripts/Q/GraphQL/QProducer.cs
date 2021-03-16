using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QProducerFragment
{
    public const string data = @"
        fragment producerData on Producer {
          id
          sim
          steps
          x
          y
          yRot
          type { id }
          material { ...resourceData }
          products { ...resourceData }
          stoppageSurchargeFactor
        }";
    
    public static readonly string withIncludes = @$"
      {QResourceFragment.data}
      {data}
    ";
}

[UsedImplicitly]
public class QProducer : QEntity
{
    [UsedImplicitly] public QProducerTypeEnum type;
    [UsedImplicitly] public List<QResource> material;
    [UsedImplicitly] public List<QResource> products;
    [UsedImplicitly] public float stoppageSurchargeFactor;
    public override string ToString()
    {
        return @$"{{
            {base.ToString()}
            type: {type}
            material: [{string.Join(",", material)}]
            products: [{string.Join(",", products)}]
            stoppageSurchargeFactor: {stoppageSurchargeFactor}
        }}";
    }
}