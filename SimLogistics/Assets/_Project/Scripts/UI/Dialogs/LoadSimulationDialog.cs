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
    [SerializeField] private Button buttonItemPrefab;
    [SerializeField] private Transform listHost;

    public ConnectionManager ConnectionManager { get; set; }
    public SimulationManager SimulationManager { get; set; }

    private void Start()
    {
        if (!SimulationManager.IsDefaultCurrent &&
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

            var spinner = UIManager.ShowSpinner();

            var nameFilter = nameInputField.text;
            var agentEndpointFilter = agentEndpointInputField.text;

            clearFiltersButton.interactable = !string.IsNullOrEmpty(nameFilter) ||
                                              !string.IsNullOrEmpty(agentEndpointFilter);

            SimulationManager.List(
                nameFilter,
                agentEndpointFilter,
                simulations =>
                {
                    spinner.Hide();

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

                        var text = $"{simulation.name} @ {AgentEndpointDisplayName(simulation.agentEndpoint)}";
                        var buttonItem = Instantiate(buttonItemPrefab, listHost.transform, false);
                        buttonItem.GetComponentInChildren<TMP_Text>().text = text;
                        buttonItem.onClick.AddListener(() =>
                        {
                            spinner = UIManager.ShowSpinner();

                            SimulationManager.Load(simulation.id, loaded =>
                            {
                                spinner.Hide();
                                Hide();
                            });
                        });
                    }

                    resultCountText.text = resultCount.ToString(CultureInfo.CurrentCulture);
                });
    }

    private string AgentEndpointDisplayName(string agentEndpoint)
    {
        if (agentEndpoint == null) return "Interactive";

        // "agentEndpoint": "https://lastknowngood.knowledge.maana.io:8443/service/maana-sim-logistics-ai-agent-v3/graphql",
        var parts = agentEndpoint.Split('/');
        return parts.Length == 6 ? parts[4] : agentEndpoint;
    }

    private void ClearList()
    {
        foreach (Transform buttonItem in listHost) Destroy(buttonItem.gameObject);
    }
}