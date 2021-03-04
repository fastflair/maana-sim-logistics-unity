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
    public string id;
}

[UsedImplicitly]
public class QProducer : QEntity
{
    public QProducerTypeEnum type;
    public List<QResource> material;
    public List<QResource> products;
    public float stoppageSurchargeFactor;
}