using System;
using System.Collections.Generic;
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
    public UnityEvent onUpdated;
    public UnityEvent<string> onSimulationError;
    
    public QState CurrentState { get; private set; }
    public QSimulation CurrentSimulation => CurrentState == null ? DefaultSimulation : CurrentState.sim;
    public string AgentEndpoint => CurrentSimulation.agentEndpoint;
    
    public bool IsDefaultCurrent => CurrentState == null || CurrentSimulation.id == DefaultSimulation.id;

    // TODO: get from simulation
    public static string MapName => "Default";
    
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

        InternalStateQueryWithLoading(queryName, query, callback);
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
            connectionManager.ApiEndpoint,
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
        
        const string queryName = "deleteSimulation";
        var query = @$"
          mutation {{
            {queryName}(id: ""{id}"") {{
              id
            }}
          }}
        ";

        connectionManager.QueryRaiseOnError<QSimulation>(
            connectionManager.ApiEndpoint,
            query,
            queryName,
            simulation =>
            {
                NotBusy();
                if (id == CurrentSimulation.id) LoadDefault();
                callback(simulation);
            });
    }

    public void MoveVehicleTo(string vehicle, float destX, float destY, Action<string> callback)
    {
        Busy();
        
        const string queryName = "moveVehicleTo";
        var query = @$"
          {QVehicleFragment.withIncludes}
          mutation {{
            {queryName}(
              sim: ""{CurrentSimulation.id}""
              vehicle: ""{vehicle}""
              destX: {destX}
              destY: {destY}
            ) {{
              ...vehicleData
            }}
          }}
        ";

        connectionManager.QueryRaiseOnError<QVehicle>(
            connectionManager.ApiEndpoint,
            query,
            queryName,
            updatedVehicle =>
            {
                NotBusy();
                callback(updatedVehicle.transitOrder.status.id);
            });
    }

    public void Transfer()
    {
        
    }

    public void Repair()
    {
        
    }
    
    public void Step(Action<QState> callback)
    {
        const string queryName = "stepSimulation";
        var query = @$"
          {QStateFragment.withIncludes}
          mutation {{
            {queryName}(sim: ""{CurrentSimulation.id}"") {{
              ...stateData
            }}
          }}
        ";

        InternalStateQueryWithBusy(queryName, query, state =>
        {
            CurrentState = state;
            onUpdated.Invoke();
        });
    }

    public void Think()
    {
        Busy();
        
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

        InternalStateQueryWithLoading(queryName, query, callback);
    }

    private void InternalStateQueryWithLoading(string queryName, string query, Action<QState> callback)
    {
        onLoading.Invoke();
        InternalStateQuery(
            queryName,
            query,
            state =>
            {
                CurrentState = state;
                onLoaded.Invoke();
                callback(CurrentState);
            });
    }
    
    private void InternalStateQueryWithBusy(string queryName, string query, Action<QState> callback)
    {
        Busy();
        InternalStateQuery(
            queryName,
            query,
            state =>
            {
                NotBusy();
                callback(CurrentState);
            });
    }

    private void InternalStateQuery(string queryName, string query, Action<QState> callback)
    {
        connectionManager.QueryRaiseOnError<QState>(
            connectionManager.ApiEndpoint,
            query,
            queryName,
            state =>
            {
                CurrentState = state;
                callback(CurrentState);
            });
    }

    private void Busy()
    {
        onBusy.Invoke();
    }
    
    private void NotBusy()
    {
        onNotBusy.Invoke();
    }
}