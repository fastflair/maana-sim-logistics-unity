using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;

public class ScenarioEditor : UIElement
{
    [SerializeField] public ConnectionManager connectionManager;
    [SerializeField] public SimulationManager simulationManager;
    [SerializeField] public UIManager uiManager;

    [SerializeField] private string defaultScenario = "Default";
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text scenarioText;
    [SerializeField] private TMP_Text serverText;
    [SerializeField] private CityPage cityPage;

    public const string SecretKey = "C=64";

    public string Scenario { get; private set; }

    public string Status
    {
        get => statusText.text;
        set => statusText.text = value;
    }

    public void Show(string scenario)
    {
        SetVisible(true, Effect.Fade);

        Scenario = scenario ?? defaultScenario;

        scenarioText.text = $"Scenario: {Scenario}";
        serverText.text = $"Server: {connectionManager.Server.Client.URL}";

        statusText.text = "Loading data...";

        LoadCities();
        // LoadProducers();
        // LoadHubs();
        // LoadVehicles();
    }

    public void LoadCities()
    {
        simulationManager.LoadCities(Scenario, cities =>
        {
            cityPage.OnEntitiesLoaded(cities);
        });
    }

    public void LoadProducers()
    {
        simulationManager.LoadProducers(Scenario, producers => {  });
    }

    public void LoadHubs()
    {
        simulationManager.LoadHubs(Scenario, hubs => { });
    }


    public void LoadVehicles()
    {
        simulationManager.LoadVehicles(Scenario, vehicles => {  });
    }

    public void OnProgressDone()
    {
        statusText.text = "Ready.";
    }

    public void OnHide()
    {
        SetVisible(false, Effect.Fade).setOnComplete(() => gameObject.SetActive(false));
    }
}