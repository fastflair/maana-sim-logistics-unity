using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadSimulationDialog : Dialog
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField agentEndpointInputField;
    [SerializeField] private Button clearFiltersButton;
    [SerializeField] private Button applyFiltersButton;
    [SerializeField] private TMP_Text resultCountText;
    [SerializeField] private ButtonItem buttonItemPrefab;
    [SerializeField] private Transform listHost;

    public ConnectionManager ConnectionManager { get; set; }
    public SimulationManager SimulationManager { get; set; }

    private void Start()
    {
        if (!SimulationManager.IsCurrentDefault &&
            SimulationManager.CurrentSimulation.agentEndpoint != null)
            agentEndpointInputField.text = SimulationManager.CurrentSimulation.agentEndpoint;

        nameInputField.onSubmit.AddListener(value => { PopulateList(); });
        agentEndpointInputField.onSubmit.AddListener(value => { PopulateList(); });

        PopulateList();
    }

    public void OnCancel()
    {
        Destroy();
    }

    public void OnClearFilters()
    {
        nameInputField.text = null;
        agentEndpointInputField.text = null;

        PopulateList();
    }
    
    public void UpdateButtons(string text)
    {
        applyFiltersButton.interactable = 
            clearFiltersButton.interactable = 
                !string.IsNullOrEmpty(text) || 
                !string.IsNullOrEmpty(nameInputField.text) || 
                !string.IsNullOrEmpty(agentEndpointInputField.text);
        
        print($"UpdateButtons: {text} {clearFiltersButton.interactable}");
    }

    public void PopulateList()
    {
        ClearList();
        
        var nameFilter = nameInputField.text;
        var agentEndpointFilter = agentEndpointInputField.text;

        UpdateButtons(null);
        
        SimulationManager.List(
            nameFilter,
            agentEndpointFilter,
            simulations =>
            {
                if (simulations.Count == 0)
                {
                    resultCountText.text = "(no results)";
                    return;
                }

                var resultCount = simulations.Count;

                foreach (var simulation in simulations)
                {
                    if (simulation.id == SimulationManager.DefaultSimulation.id)
                    {
                        resultCount--;
                        continue;
                    }

                    var text = SimulationManager.FormatSimulationDisplay(simulation);
                    var buttonItem = Instantiate(buttonItemPrefab, listHost.transform, false);
                    buttonItem.Label = text;
                    buttonItem.onClick.AddListener(() =>
                    {
                        SimulationManager.Load(simulation.id, loaded =>
                        {
                            Destroy();
                        });
                    });
                }

                resultCountText.text = resultCount.ToString(CultureInfo.CurrentCulture);
            });
    }
    
    private void ClearList()
    {
        foreach (Transform buttonItem in listHost) Destroy(buttonItem.gameObject);
        resultCountText.text = 0.ToString();
    }
}