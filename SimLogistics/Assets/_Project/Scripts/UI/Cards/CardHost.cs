using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardHost : MonoBehaviour
{
    [SerializeField] protected TMP_Text title;
    [SerializeField] protected Image thumbnail;
    [SerializeField] protected CityCard cityCard;
    [SerializeField] protected ProducerCard producerCard;
    [SerializeField] protected HubCard hubCard;
    [SerializeField] protected VehicleCard vehicleCard;
    [SerializeField] protected WarehouseCard warehouseCard;
    
    private UIElement _activeCard;
    public void ShowCardForEntity(Entity.EntityType type, QEntity qEntity)
    {
        var prevActiveCard = _activeCard;
        
        switch (type)
        {
            case Entity.EntityType.City:
                _activeCard = cityCard;
                cityCard.Populate((QCity)qEntity);
                break;
            case Entity.EntityType.Producer:
                var qProducer = (QProducer) qEntity;
                if (qProducer.type.id == "Warehouse")
                {
                    _activeCard = warehouseCard;
                    warehouseCard.Populate(qProducer);
                }
                else
                {
                    _activeCard = producerCard;
                    producerCard.Populate(qProducer);
                }

                break;
            case Entity.EntityType.Hub:
                _activeCard = hubCard;
                hubCard.Populate((QHub)qEntity);
                break;
            case Entity.EntityType.Vehicle:
                _activeCard = vehicleCard;
                vehicleCard.Populate((QVehicle)qEntity);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (_activeCard == prevActiveCard) return;
        _activeCard.Show();
        if (prevActiveCard) prevActiveCard.Hide();
    }
    
    public void SetEntityId(string id)
    {
        title.text = SimulationManager.FormatEntityIdDisplay(id);
    }

    public void SetThumbnail(Sprite thumbnailSprite)
    {
        thumbnail.sprite = thumbnailSprite;
    }
}