using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VehicleInfoItem : MonoBehaviour
{
    [SerializeField] private Image thumbnail;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private TMP_Text locationText;
    [SerializeField] private TMP_Text fuelText;
    [SerializeField] private TMP_Text transitText;
    [SerializeField] private TMP_Text serviceText;

    [SerializeField] private Sprite truckSprite;
    [SerializeField] private Sprite planeSprite;
    [SerializeField] private Sprite shipSprite;

    public Sprite Thumbnail
    {
        get => thumbnail.sprite;
        set => thumbnail.sprite = value;
    }
    public string Label
    {
        get => labelText.text;
        set => labelText.text = value;
    }

    public string Location
    
    {
        get => locationText.text;
        set => locationText.text = value;
    }
    public string Fuel
    {
        get => fuelText.text;
        set => fuelText.text = value;
    }
    public string Transit
    {
        get => transitText.text;
        set => transitText.text = value;
    }
    public string Service
    {
        get => serviceText.text;
        set => serviceText.text = value;
    }

    public void Populate(QVehicle vehicle)
    {
        Thumbnail = vehicle.type.id switch
        {
            "Truck" => truckSprite,
            "Plane" => planeSprite,
            "Ship" => shipSprite,
            _ => Thumbnail
        };
        Label = SimulationManager.FormatEntityIdDisplay(vehicle.id);
        Location = $"({vehicle.x}, {vehicle.y})";
        Transit = vehicle.transitOrder.status.id;
        Fuel = $"{vehicle.fuel.quantity}/{vehicle.fuel.capacity} @ {vehicle.fuel.consumptionRate}/step";
        Service = $"{vehicle.lastServiceStep}/{vehicle.serviceInterval}";
    }
}
