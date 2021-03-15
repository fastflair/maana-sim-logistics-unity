using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransitOrderItem : MonoBehaviour
{
    [SerializeField] private TMP_Text vehicleText;
    [SerializeField] private TMP_Text destinationText;
    [SerializeField] private TMP_Text stepsText;
    [SerializeField] private TMP_Text statusText;

    public VehicleManager VehicleManager { get; set; }

    private QTransitOrder _transitOrder;

    public string Vehicle
    {
        get => vehicleText.text;
        set => vehicleText.text = value;
    }

    public string Destination
    {
        get => destinationText.text;
        set => destinationText.text = value;
    }

    public string Steps
    {
        get => stepsText.text;
        set => stepsText.text = value;
    }

    public string Status
    {
        get => statusText.text;
        set => statusText.text = value;
    }

    public void Populate(QTransitOrder transitOrder)
    {
        // print($"Populating transit order item: {transitOrder}");
        _transitOrder = transitOrder;
        Vehicle = SimulationManager.FormatEntityIdDisplay(transitOrder.vehicle);
        if (transitOrder.waypoints.Count == 0)
        {
            try
            {
                var vehicle = VehicleManager.QEntities.First(x => x.id == transitOrder.vehicle);
                Destination = $"({vehicle.x}, {vehicle.y})";
            }
            catch
            {
                Destination = "n/a";
            }
        }
        else
        {
            Destination = SimulationManager.FormatWaypointDisplay(transitOrder.waypoints);
        }

        Steps = transitOrder.steps.ToString(CultureInfo.CurrentCulture);
        Status = transitOrder.status.id;
    }

    public void OnClick()
    {
        VehicleManager.SelectEntity(_transitOrder.vehicle);
    }
}