using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ProducerCard : Card
{
    [SerializeField] protected Transform materialItemList;
    [SerializeField] protected Transform productItemList;
    [SerializeField] protected Transform propertyItemList;
    [SerializeField] protected Transform noteItemList;
    
    public void Populate(QProducer producer)
    {
        // print($"Populate producer: {producer}");
        
        ClearLists();
        
        host.SetEntityId(producer.id);
        host.SetThumbnail(host.producerManager.EntityThumbnail(producer.type.id));

        AddPropertyToList(propertyItemList, "Stoppage Surcharge Factor", producer.stoppageSurchargeFactor.ToString(CultureInfo.CurrentCulture));
        
        foreach (var resource in producer.material)
            AddResourceToList(materialItemList, resource);

        foreach (var resource in producer.products)
        {
            AddResourceToList(productItemList, resource);
        }
        
        AddTransferOrdersToList(producer.id);
    }
    
    private void ClearLists()
    {
        ClearList(materialItemList);
        ClearList(productItemList);
        ClearList(propertyItemList);
        ClearList(noteItemList);
    }
}