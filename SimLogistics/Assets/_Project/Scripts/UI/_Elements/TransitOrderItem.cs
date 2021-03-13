using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransitOrderItem : MonoBehaviour
{
    [SerializeField] private TMP_Text vehicleText;
    [SerializeField] private TMP_Text destinationText;
    [SerializeField] private TMP_Text stepsText;
    [SerializeField] private TMP_Text statusText;

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

    public void Populate(QTransitOrder transit)
    {
        Vehicle = SimulationManager.FormatEntityIdDisplay(transit.vehicle);
        Destination = $"({transit.destX}, {transit.destY})";
        Steps = transit.steps.ToString(CultureInfo.CurrentCulture);
        Status = transit.status.id;
    }
}
