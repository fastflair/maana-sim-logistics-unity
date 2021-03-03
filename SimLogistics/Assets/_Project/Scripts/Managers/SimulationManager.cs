using System;
using System.Collections.Generic;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;

public class SimulationManager : MonoBehaviour
{
    private const string SimulationData = @"
        fragment simulationData on Simulation {
          id
          name
          agentEndpoint
          steps
          balance
          income
          expenses
        }";

    [SerializeField] private ConnectionManager connectionManager;

    public UnityEvent onDefaultSimulation;
    public UnityEvent onNewSimulation;
    public UnityEvent onSimulationBusy;
    public UnityEvent<string> onSimulationError;

    public GraphQLManager agentEndpointClient;

    public readonly QSimulation DefaultSimulation = new QSimulation
    {
        id = "Default",
        name = "Default",
        agentEndpoint = null
    };

    public QSimulation CurrentSimulation { get; private set; }

    public bool IsDefaultCurrent => CurrentSimulation.id == DefaultSimulation.id;

    public void OnConnected()
    {
        SetDefault();
    }

    private void SetDefault()
    {
        CurrentSimulation = DefaultSimulation;
        onDefaultSimulation.Invoke();
    }
    
    public void New(string simName, string agentEndpoint, Action<QSimulation> callback)
    {
        const string queryName = "newSimulation";
        var query = @$"
          {SimulationData}
          mutation {{
            {queryName}(name: ""{simName}"", agentEndpoint: ""{agentEndpoint}"") {{
              ...simulationData
            }}
          }}
        ";
        
        connectionManager.QueryRaiseOnError<QSimulation>(
            connectionManager.apiEndpoint,
            query, 
            queryName,
            simulation =>
            {
                CurrentSimulation = simulation;
                onNewSimulation.Invoke();
                callback(simulation);
            });
    }

    public void Overwrite(string simName, string agentEndpoint, Action<QSimulation> callback)
    {
        const string queryName = "overwriteSimulation";
        var query = @$"
          {SimulationData}
          mutation {{
            {queryName}(name: ""{simName}"", agentEndpoint: ""{agentEndpoint}"") {{
              ...simulationData
            }}
          }}
        ";
        
        connectionManager.QueryRaiseOnError<QSimulation>(
            connectionManager.apiEndpoint,
            query, 
            queryName,
            simulation =>
            {
                CurrentSimulation = simulation;
                onNewSimulation.Invoke();
                callback(simulation);
            });
    }

    public void Load(string id, Action<QSimulation> callback)
    {
        const string queryName = "loadSimulation";
        var query = @$"
          {SimulationData}
          query {{
            {queryName}(id: ""{id}"") {{
              ...simulationData
            }}
          }}
        ";
        
        connectionManager.QueryRaiseOnError<QSimulation>(
            connectionManager.apiEndpoint,
            query, 
            queryName,
            simulation =>
            {
                CurrentSimulation = simulation;
                onNewSimulation.Invoke();
                callback(simulation);
            });
    }

    public void List(string simName, string agentEndpoint, Action<List<QSimulation>> callback)
    {
        const string queryName = "selectSimulation";
        var query = @$"
          {SimulationData}
          query {{
            {queryName}(name: ""{simName}"", agentEndpoint: ""{agentEndpoint}"") {{
              ...simulationData
            }}
          }}
        ";

        connectionManager.QueryRaiseOnError(
            connectionManager.apiEndpoint,
            query,
            queryName,
            callback);
    }

    public void Delete(string id, Action<QSimulation> callback)
    {
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
                if (id == CurrentSimulation.id)
                {
                    SetDefault();
                }
                callback(simulation);
            });
    }

    public void Think()
    {
        Debug.Assert(agentEndpointClient != null);
    }
}