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
    public UnityEvent onActionsUpdated;
    public UnityEvent onUpdated;
    public UnityEvent<string> onSimulationError;

    public QState CurrentState { get; private set; }
    public QSimulation CurrentSimulation => CurrentState == null ? DefaultSimulation : CurrentState.sim;
    public string AgentEndpoint => CurrentSimulation.agentEndpoint;
    public bool IsCurrentDefault => CurrentState == null || CurrentSimulation.id == DefaultSimulation.id;

    public QActions Actions { get; private set; } = new QActions();

    // TODO: get from simulation
    public static string MapName => "Default";

    public static string FormatEntityId(string id)
    {
        var parts = id.Split('/');
        if (parts.Length > 1)
        {
            id = parts[1];
        }

        parts = id.Split(':');
        if (parts.Length > 1)
        {
            id = $"{parts[0]} {parts[1]}";
        }

        return id;
    }

    public static string FormatDisplayText(QSimulation simulation)
    {
        return $"{simulation.name} @ {AgentEndpointDisplayName(simulation.agentEndpoint)} - Steps: {simulation.steps}";
    }

    public static string AgentEndpointDisplayName(string agentEndpoint)
    {
        return agentEndpoint ?? "Interactive";
    }
    
    // Actions
    // -------
    
    public void ResetActions()
    {
        Actions = new QActions();
        onActionsUpdated.Invoke();
    }
    
    public QTransitAction AddTransitAction(string vehicle, float destX, float destY)
    {
        var transitAction = new QTransitAction
        {
            id = $"{CurrentSimulation.id}:{vehicle}",
            vehicle = vehicle,
            destX = destX,
            destY = destY
        };
        Actions.transitActions.Add(transitAction);
        onActionsUpdated.Invoke();
        return transitAction;
    }

    public void RemoveTransitAction(QTransitAction transitAction)
    {
        Actions.transitActions.Remove(transitAction);
        onActionsUpdated.Invoke();
    }
    
    public QRepairAction AddRepairAction(
        string vehicle,
        string hub)
    {
        var repairAction = new QRepairAction()
        {
            id = $"{CurrentSimulation.id}:{vehicle}",
            sim = CurrentSimulation.id,
            vehicle = vehicle,
            hub = hub
        };
        Actions.repairActions.Add(repairAction);
        onActionsUpdated.Invoke();
        return repairAction;
    }

    public void RemoveRepairAction(QRepairAction repairAction)
    {
        Actions.repairActions.Remove(repairAction);
        onActionsUpdated.Invoke();
    }

    public QTransferAction AddTransferAction(
        string vehicle,
        string counterparty,
        QResourceTypeEnum resourceType,
        float quantity,
        QResourceTransferTypeEnum transferType)
    {
        var transferAction = new QTransferAction()
        {
            id = $"{CurrentSimulation.id}:{vehicle}:{counterparty}",
            sim = CurrentSimulation.id,
            vehicle = vehicle,
            counterparty =  counterparty,
            resourceType = resourceType,
            quantity = quantity,
            transferType = transferType
        };
        
        Actions.transferActions.Add(transferAction);
        onActionsUpdated.Invoke();
        return transferAction;
    }
    
    public void RemoveTransferAction(QTransferAction transferAction)
    {
        Actions.transferActions.Remove(transferAction);
        onActionsUpdated.Invoke();    
    }

    // Event handlers
    // --------------
    
    public void OnConnected()
    {
        LoadDefault();
        ResetActions();
    }

    // Simulation management
    // ---------------------
    
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
                print($"List of simulations: {string.Join(",", res)}");

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
                print($"Deleted simulation: {simulation}");

                NotBusy();
                if (id == CurrentSimulation.id) LoadDefault();
                callback(simulation);
            });
    }
    
    public void Think(string agentEndpoint, QState state, Action<QActions> callback)
    {
        Busy();
        print($"Agent endpoint: {agentEndpoint}");
        print($"Think about: {state}");

        const string queryName = "think";
        var query = @$"
          {QActionsFragment.withIncludes}
          mutation {{
            {queryName}(state: {state}) {{
              ...actionsData
            }}
          }}
        ";

        connectionManager.QueryRaiseOnError<QActions>(
            agentEndpoint,
            query,
            queryName,
            actions =>
            {
                NotBusy();

                print($"Think actions: {actions}");

                Actions = actions;
                onActionsUpdated.Invoke();

                callback.Invoke(actions);
            });
    }

    public void Simulate(Action<QState> callback)
    {
        Busy();
        
        IssueActions(Actions, success =>
        {
            if (!success)
            {
                NotBusy();
                callback.Invoke(null);
            }
            
            const string queryName = "stepSimulation";
            var query = @$"
              {QStateFragment.withIncludes}
              mutation {{
                {queryName}(sim: ""{CurrentSimulation.id}"") {{
                  ...stateData
                }}
              }}
            ";

            InternalStateQuery(queryName, query, state =>
            {
                // print($"Step results: {state}");
                NotBusy();
                CurrentState = state;
                onUpdated.Invoke();
            });
        });
    }

    // --- Internal

    private void IssueActions(QActions actions, Action<bool> callback)
    {
        const string queryName = "issueActions";
        var query = @$"
          mutation {{
            {queryName}(actions: {actions})
          }}
        ";

        connectionManager.QueryRaiseOnError<bool>(
            connectionManager.ApiEndpoint,
            query,
            queryName,
            success =>
            {
                print($"Issue actions: {success}");

                if (success) ResetActions();
                
                callback.Invoke(success);
            });
    }
    
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
                print($"{queryName} results: {state}");

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