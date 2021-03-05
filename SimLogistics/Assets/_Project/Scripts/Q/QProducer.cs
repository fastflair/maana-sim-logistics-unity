using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QProducerFragment
{
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

public class QProducerTypeEnum
{
    [UsedImplicitly] public string id;
}

[UsedImplicitly]
public class QProducer : QEntity
{
    [UsedImplicitly] public QProducerTypeEnum type;
    [UsedImplicitly] public List<QResource> material;
    [UsedImplicitly] public List<QResource> products;
    [UsedImplicitly] public float stoppageSurchargeFactor;
}