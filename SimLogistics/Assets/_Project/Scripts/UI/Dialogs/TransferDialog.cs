using System.Globalization;
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
    
    public UnityEvent onOkay = new UnityEvent();
    
    public SimulationManager SimulationManager { get; set; }

    public QVehicle Vehicle;
    public QProducer Producer;
    public QCity City;

    public float Quantity => float.Parse(quantityInputField.text);
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
        quantityInputField.text = 0.ToString();
        
        PopulateList();

        Validate();
    }

    public void OnOkay()
    {
        Hide();
        
        onOkay.Invoke();
    }

    public void OnCancel()
    {
        Hide();
    }

    private void PopulateList()
    {
        ClearList();
        
        var item = Instantiate(toggleItemPrefab, listHost.transform, false);
        item.Label = "This is a test"; //SimulationManager.FormatDisplayText(simulation);
    }
    
    private void ClearList()
    {
        foreach (Transform item in listHost) Destroy(item.gameObject);
    }

    private void Validate()
    {
        okayButton.interactable = false;
    }
}