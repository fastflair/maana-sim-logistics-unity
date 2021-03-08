using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ProducerCard : Card
{
    [SerializeField] private CardHost host;
    
    public void Populate(QProducer qProducer)
    {
        host.SetEntityId(qProducer.id);
        host.SetThumbnail(producerManager.EntityThumbnail(qProducer.type.id));

        // AddPropertyToGroup(propertyGroup1, "Stoppage Surcharge Factor", qProducer.stoppageSurchargeFactor.ToString(CultureInfo.CurrentCulture));
        //
        // foreach (var resource in qProducer.material)
        //     AddResourceToGroup(propertyGroup2, resource);
        //
        // foreach (var resource in qProducer.products)
        //     AddResourceToGroup(propertyGroup3, resource);
    }
}