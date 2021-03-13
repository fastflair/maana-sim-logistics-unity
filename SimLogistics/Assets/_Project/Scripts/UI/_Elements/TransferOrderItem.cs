using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransferOrderItem : MonoBehaviour
{
    [SerializeField] private TMP_Text vehicleText;
    [SerializeField] private TMP_Text counterpartyText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text detailText;

    public string Vehicle
    {
        get => vehicleText.text;
        set => vehicleText.text = value;
    }
    public string Counterparty
    {
        get => counterpartyText.text;
        set => counterpartyText.text = value;
    }
    public string Status
    {
        get => statusText.text;
        set => statusText.text = value;
    }
    public string Detail
    {
        get => detailText.text;
        set => detailText.text = value;
    }

    public void Populate(QResourceTransfer transfer)
    {
        Vehicle = SimulationManager.FormatEntityIdDisplay(transfer.vehicle);
        Counterparty = SimulationManager.FormatEntityIdDisplay(transfer.counterparty);
        Status = transfer.status.id;
        Detail = SimulationManager.FormatTransferDetailDisplay(transfer);
    }
}
