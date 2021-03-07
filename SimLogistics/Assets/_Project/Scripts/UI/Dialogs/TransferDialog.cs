using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TransferDialog : Dialog
{
    [SerializeField] private TMP_InputField quantityInputField;
    [SerializeField] private ToggleItem toggleItemPrefab;
    [SerializeField] private Transform listHost;
    [SerializeField] private Button okayButton;
    [SerializeField] private ToggleGroup transferTypeToggleGroup;
    [SerializeField] private ToggleGroup resourceTypeToggleGroup;

    public UnityEvent onAddTransfer = new UnityEvent();

    public SimulationManager SimulationManager { get; set; }

    private QVehicle _vehicle;
    public QVehicle Vehicle
    {
        get => _vehicle;
        set
        {
            _vehicle = value;
            PopulateList();
        }
    }

    private QCity _city;
    public QCity City
    {
        get => _city;
        set
        {
            _city = value;
            PopulateList();
        }
    }

    private QProducer _producer;
    public QProducer Producer
    {
        get => _producer;
        set
        {
            _producer = value;
            PopulateList();
        }
    }

    public float Quantity
    {
        get
        {
            var text = quantityInputField.text;
            return string.IsNullOrEmpty(text) ? 0f : float.Parse(text);
        }
    }

    public string TransferType =>
        transferTypeToggleGroup
            .GetFirstActiveToggle()
            .GetComponent<ToggleItem>()
            .Label;

    public string ResourceType =>
        resourceTypeToggleGroup
            .GetFirstActiveToggle()
            .GetComponent<ToggleItem>()
            .data as string;

    private void Start()
    {
        quantityInputField.text = 0f.ToString(CultureInfo.CurrentCulture);
    }

    public void OnOkay()
    {
        onAddTransfer.Invoke();
        Hide();
    }

    public void OnCancel()
    {
        Hide();
    }

    public void PopulateList()
    {
        if (Vehicle == null || City == null && Producer == null) return;

        ClearList();
        
        var resources = TransferType is "Deposit"
            ? SimulationManager.GetVehicleCargo(Vehicle.id)
            : City != null
                ? SimulationManager.GetCityDemands(City.id)
                : SimulationManager.GetProducerProducts(Producer.id);

        bool firstSet = false;
        foreach (var resource in resources)
        {
            var item = Instantiate(toggleItemPrefab, listHost.transform, false);
            item.data = resource.type.id;
            item.Label = $"{resource.type.id}: {SimulationManager.FormatResourceDetailDisplay(resource)}";
            if (!firstSet)
            {
                item.IsOn = true;
                firstSet = true;
            }
            else
            {
                item.IsOn = false;
            }
            item.Group = resourceTypeToggleGroup;
            item.onValueChanged.AddListener(ValidateForm);
            print($"resource: {item.data} {item.Label}");
        }

        ValidateForm();
    }

    public void ValidateForm()
    {
        print($"Validate: resourceType = [{ResourceType}], Quantity: [{Quantity}]");
        okayButton.interactable = Quantity > 0f && ResourceType != null;
    }

    private void ClearList()
    {
        foreach (Transform item in listHost) Destroy(item.gameObject);
    }

}