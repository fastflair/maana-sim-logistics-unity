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

    public CityManager  CityManager { get; set; }
    public ProducerManager  ProducerManager { get; set; }
    public HubManager  HubManager { get; set; }
    public VehicleManager  VehicleManager { get; set; }

    private QResourceTransfer _transfer;
    
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
        _transfer = transfer;
        Vehicle = SimulationManager.FormatEntityIdDisplay(transfer.vehicle);
        Counterparty = SimulationManager.FormatEntityIdDisplay(transfer.counterparty);
        Status = transfer.status.id;
        Detail = SimulationManager.FormatTransferDetailDisplay(transfer);
    }
    
    public void OnClick()
    {        
        var counterpartyEntity = ProducerManager.Entity(this._transfer.counterparty);
        if (counterpartyEntity == null)
        {
            counterpartyEntity = CityManager.Entity(_transfer.counterparty);
            if (counterpartyEntity == null)
            {
                counterpartyEntity = HubManager.Entity(_transfer.counterparty);
                if (counterpartyEntity == null)
                {
                    return;
                }
            }
        }
        var entities = new[] { VehicleManager.Entity(_transfer.vehicle), counterpartyEntity};
        VehicleManager.SelectEntities(entities);
    }

}
