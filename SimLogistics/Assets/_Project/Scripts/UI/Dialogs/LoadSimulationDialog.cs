using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadSimulationDialog : Dialog
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField agentEndpointInputField;
    [SerializeField] private Button clearFiltersButton;
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
        Hide();
    }

    public void OnClearFilters()
    {
        nameInputField.text = null;
        agentEndpointInputField.text = null;

        PopulateList();
    }
    
    private void PopulateList()
    {
        ClearList();
        
        var nameFilter = nameInputField.text;
        var agentEndpointFilter = agentEndpointInputField.text;

        clearFiltersButton.interactable = !string.IsNullOrEmpty(nameFilter) ||
                                          !string.IsNullOrEmpty(agentEndpointFilter);

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

                    var text = SimulationManager.FormatDisplayText(simulation);
                    var buttonItem = Instantiate(buttonItemPrefab, listHost.transform, false);
                    buttonItem.Label = text;
                    buttonItem.onClick.AddListener(() =>
                    {
                        SimulationManager.Load(simulation.id, loaded =>
                        {
                            Hide();
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