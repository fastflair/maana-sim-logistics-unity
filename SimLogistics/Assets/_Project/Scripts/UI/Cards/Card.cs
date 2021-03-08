using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : UIElement
{
    [SerializeField] protected ProducerManager producerManager;
    
    [SerializeField] protected PropertyItem propertyItemPrefab;
    [SerializeField] protected ResourceItem resourceItemPrefab;
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
        print($"[name] resource: {resource}");
        var resourceItem = Instantiate(resourceItemPrefab, list, false);
        resourceItem.Thumbnail = producerManager.ResourceThumbnail(resource.type.id);
        resourceItem.Label = resource.type.id;
        resourceItem.Volume = SimulationManager.FormatResourceVolumeDisplay(resource);
        resourceItem.Pricing = SimulationManager.FormatResourcePricingDisplay(resource);
        resourceItem.Rate = SimulationManager.FormatResourceRateDisplay(resource);
    }
}