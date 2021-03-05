using System;
using System.Collections.Generic;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;

// "agentEndpoint": "https://lastknowngood.knowledge.maana.io:8443/service/maana-sim-logistics-ai-agent-v3/graphql",

public class SimulationManager : MonoBehaviour
{
    public static readonly QSimulation DefaultSimulation = new QSimulation
    {
        id = "Default",
        name = "Default",
        agentEndpoint = null
    };

    [SerializeField] private ConnectionManager connectionManager;

    public UnityEvent onLoading;
    public UnityEvent onLoaded;
    public UnityEvent onBusy;
    public UnityEvent onNotBusy;
    public UnityEvent<string> onSimulationError;
    
    public QState CurrentState { get; private set; }
    public QSimulation CurrentSimulation => CurrentState == null ? DefaultSimulation : CurrentState.sim;

    public bool IsDefaultCurrent => CurrentState == null || CurrentSimulation.id == DefaultSimulation.id;

    public void OnConnected()
    {
        LoadDefault();
    }

    public void New(string simName, string agentEndpoint, Action<QState> callback)
    {
        const string queryName = "newSimulation";
        InternalNew(queryName, simName, agentEndpoint, callback);
    }

    public void Overwrite(string simName, string agentEndpoint, Action<QState> callback)
    {
        const string queryName = "overwriteSimulation";
        InternalNew(queryName, simName, agentEndpoint, callback);
    }

    public void Load(string id, Action<QState> callback)
    {
        const string queryName = "loadState";
        var query = @$"
          {QStateFragment.withIncludes}
          query {{
            {queryName}(sim: ""{id}"") {{
              ...stateData
            }}
          }}
        ";

        InternalQueryNew(queryName, query, callback);
    }

    public void List(string simName, string agentEndpoint, Action<List<QSimulation>> callback)
    {
        Busy();
        
        const string queryName = "selectSimulation";
        var query = @$"
          {QSimulationFragment.data}
          query {{
            {queryName}(name: ""{simName}"", agentEndpoint: ""{agentEndpoint}"") {{
              ...simulationData
            }}
          }}
        ";

        connectionManager.QueryRaiseOnError<List<QSimulation>>(
            connectionManager.apiEndpoint,
            query,
            queryName,
            res =>
            {
                NotBusy();
                callback(res);
            });
    }

    public void Delete(string id, Action<QSimulation> callback)
    {
        Busy();
        
        var queryName = "deleteSimulation";
        var query = @$"
          mutation {{
            {queryName}(id: ""{id}"") {{
              id
            }}
          }}
        ";

        connectionManager.QueryRaiseOnError<QSimulation>(
            connectionManager.apiEndpoint,
            query,
            queryName,
            simulation =>
            {
                NotBusy();
                if (id == CurrentSimulation.id) LoadDefault();
                callback(simulation);
            });
    }

    public void Think()
    {
        Busy();
        // Debug.Assert(agentEndpointClient != null);
        NotBusy();
    }

    // --- Internal
    
    private void LoadDefault()
    {
        Load(DefaultSimulation.id, state => { });
    }

    private void InternalNew(string queryName, string simName, string agentEndpoint, Action<QState> callback)
    {
        var query = @$"
          {QStateFragment.withIncludes}
          mutation {{
            {queryName}(name: ""{simName}"", agentEndpoint: ""{agentEndpoint}"") {{
              ...stateData
            }}
          }}
        ";

        InternalQueryNew(queryName, query, callback);
    }

    private void InternalQueryNew(string queryName, string query, Action<QState> callback)
    {
        print("onLoading");
        onLoading.Invoke();
        
        connectionManager.QueryRaiseOnError<QState>(
            connectionManager.apiEndpoint,
            query,
            queryName,
            state =>
            {
                CurrentState = state;
                print($"[{queryName}] onNewSimulation: {CurrentSimulation.name}");
                onLoaded.Invoke();
                callback(CurrentState);
            });
    }
    
    private void Busy()
    {
        print("onBusy");
        onBusy.Invoke();
    }
    
    private void NotBusy()
    {
        print("onNotBusy");
        onNotBusy.Invoke();
    }
}