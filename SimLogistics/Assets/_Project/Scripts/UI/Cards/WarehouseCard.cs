using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class WarehouseCard : Card
{
    [SerializeField] private CardHost host;
    [SerializeField] private Sprite warehouseSprite;

    [SerializeField] protected Transform storageItemList;
    [SerializeField] protected Transform propertyItemList;
    [SerializeField] protected Transform noteItemList;

    public void Populate(QProducer qProducer)
    {
        ClearLists();
        
        host.SetEntityId(qProducer.id);
        host.SetThumbnail(warehouseSprite);
        
        foreach (var resource in qProducer.material)
            AddResourceToList(storageItemList, resource);
    }
    
    private void ClearLists()
    {
        ClearList(storageItemList);
        ClearList(propertyItemList);
        ClearList(noteItemList);
    }
}