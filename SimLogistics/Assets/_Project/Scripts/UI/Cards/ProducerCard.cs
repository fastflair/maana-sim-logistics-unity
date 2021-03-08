using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ProducerCard : Card
{
    [SerializeField] private CardHost host;
    [SerializeField] protected Transform materialItemList;
    [SerializeField] protected Transform productItemList;
    [SerializeField] protected Transform propertyItemList;
    [SerializeField] protected Transform noteItemList;
    
    public void Populate(QProducer qProducer)
    {
        ClearLists();
        
        host.SetEntityId(qProducer.id);
        host.SetThumbnail(producerManager.EntityThumbnail(qProducer.type.id));

        AddPropertyToList(propertyItemList, "Stoppage Surcharge Factor", qProducer.stoppageSurchargeFactor.ToString(CultureInfo.CurrentCulture));
        
        foreach (var resource in qProducer.material)
            AddResourceToList(materialItemList, resource);
        
        foreach (var resource in qProducer.products)
            AddResourceToList(productItemList, resource);
    }
    
    private void ClearLists()
    {
        ClearList(materialItemList);
        ClearList(productItemList);
        ClearList(propertyItemList);
        ClearList(noteItemList);
    }
}