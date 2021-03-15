using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : UIElement
{
    [SerializeField] protected CityManager cityManager;
    [SerializeField] protected ProducerManager producerManager;
    [SerializeField] protected HubManager hubManager;
    [SerializeField] protected VehicleManager vehicleManager;
    
    [SerializeField] protected CardHost host;
    [SerializeField] protected Transform ordersItemList;
    [SerializeField] protected ResourceItem resourceItemPrefab;
    [SerializeField] protected PropertyItem propertyItemPrefab;
    [SerializeField] protected TransferOrderItem transferOrderItemPrefab;
    [SerializeField] protected NoteItem noteItemPrefab;

    protected static void ClearList(Transform list)
    {
        foreach (Transform child in list)
            Destroy(child.gameObject);
    }

    protected void AddPropertyToList(Transform list, string label, string value)
    {
        var propertyItem = Instantiate(propertyItemPrefab, list, false);
        propertyItem.Label = label;
        propertyItem.Value = value;
    }

    protected void AddResourceToList(Transform list, QResource resource)
    {
        var resourceItem = Instantiate(resourceItemPrefab, list, false);
        print($"Resource: {resource.type.id}");
        resourceItem.Populate(resource, host.producerManager.ResourceThumbnail(resource.type.id));
    }

    protected void AddTransferOrdersToList(string counterparty)
    {
        ClearList(ordersItemList);
        
        var col = host.simulationManager.CurrentState.transfers.FindAll(x => x.counterparty == counterparty);
        col.Sort(SimulationManager.TransferComparer);
        foreach (var transferOrder in col)
        {
            var transferOrderItem = Instantiate(transferOrderItemPrefab, ordersItemList, false);
            transferOrderItem.CityManager = cityManager;
            transferOrderItem.ProducerManager = producerManager;
            transferOrderItem.HubManager = hubManager;
            transferOrderItem.VehicleManager = vehicleManager;
            transferOrderItem.Populate(transferOrder);
        }
    }
}