using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QProducerFragment
{
    public static readonly string withIncludes = @$"
      {QResourceFragment.OutputData}
      {data}
    ";
    
    public const string data = @"
        fragment producerData on ProducerOutput {
          id
          sim
          steps
          x
          y
          type { id }
          material { ...resourceData }
          products { ...resourceData }
          stoppageSurchargeFactor
        }";
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