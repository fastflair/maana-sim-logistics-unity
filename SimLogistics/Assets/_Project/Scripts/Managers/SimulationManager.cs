using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// "agentEndpoint": "https://lastknowngood.knowledge.maana.io:8443/service/maana-sim-logistics-ai-agent-v3/graphql",

public class SimulationManager : MonoBehaviour
{
    public enum ActionType
    {
        Transit,
        Repair,
        Transfer
    }

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
    public UnityEvent<ActionInfo> onNewAction;
    public UnityEvent onActionsReset;
    public UnityEvent<string> onSimulationError;

    public QState CurrentState { get; private set; }
    public QSimulation CurrentSimulation => CurrentState == null ? DefaultSimulation : CurrentState.sim;
    public string AgentEndpoint => CurrentSimulation.agentEndpoint;
    public bool IsCurrentDefault => CurrentState == null || CurrentSimulation.id == DefaultSimulation.id;

    public QActions Actions { get; private set; } = new QActions();

    // TODO: get from simulation
    public static string MapName => "Default";

    // Display formatters
    // ------------------

    public static string FormatEntityIdDisplay(string id)
    {
        var parts = id.Split('/');
        if (parts.Length > 1) id = parts[1];

        parts = id.Split(':');
        if (parts.Length > 1) id = $"{parts[0]} {parts[1]}";

        return id;
    }

    public static string FormatSimulationDisplay(QSimulation simulation)
    {
        return
            $"{simulation.name} @ {FormatAgentEndpointDisplay(simulation.agentEndpoint)} - Steps: {simulation.steps}";
    }

    public static string FormatAgentEndpointDisplay(string agentEndpoint)
    {
        return agentEndpoint ?? "Interactive";
    }


    public static string FormatResourceDetailDisplay(QResource resource)
    {
        var volume = FormatResourceVolumeDisplay(resource);
        var pricing = FormatResourcePricingDisplay(resource);
        var rate = FormatResourceRateDisplay(resource);

        return $"{volume} @ {pricing} {rate}";
    }

    public static string FormatResourceVolumeDisplay(QResource resource)
    {
        var qty = resource.quantity.ToString("F", CultureInfo.CurrentCulture);
        var cap = resource.capacity.ToString("F", CultureInfo.CurrentCulture);

        return $"{qty}/{cap}";
    }

    public static string FormatResourcePricingDisplay(QResource resource)
    {
        var bpp = resource.basePricePerUnit.ToString("C", CultureInfo.CurrentCulture);
        var app = resource.adjustedPricePerUnit.ToString("C", CultureInfo.CurrentCulture);

        return $"{app}/ea ({bpp})";
    }

    public static string FormatResourceRateDisplay(QResource resource)
    {
        var cr = resource.consumptionRate.ToString("F", CultureInfo.CurrentCulture);
        var rr = resource.replenishRate.ToString("F", CultureInfo.CurrentCulture);

        return $"↓{cr} ↑{rr}";
    }

    // Resource accessors
    // ------------------

    public IEnumerable<QResource> GetVehicleCargo(string id)
    {
        return CurrentState.vehicles.First(x => x.id == id).cargo;
    }

    public IEnumerable<QResource> GetCityDemands(string id)
    {
        return CurrentState.cities.First(x => x.id == id).demand;
    }

    public IEnumerable<QResource> GetProducerMaterial(string id)
    {
        return CurrentState.producers.First(x => x.id == id).material;
    }

    public IEnumerable<QResource> GetProducerProducts(string id)
    {
        return CurrentState.producers.First(x => x.id == id).products;
    }

    // Actions
    // -------

    public void ResetActions()
    {
        Actions = new QActions();
        onActionsReset.Invoke();
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
        onNewAction.Invoke(TransitActionInfo(transitAction));
        return transitAction;
    }

    public void RemoveTransitAction(string id)
    {
        Actions.transitActions =
            Actions
                .transitActions
                .Where(x => x.id != id)
                .ToList();
    }

    public QRepairAction AddRepairAction(
        string vehicle,
        string hub)
    {
        var repairAction = new QRepairAction
        {
            id = $"{CurrentSimulation.id}:{vehicle}",
            sim = CurrentSimulation.id,
            vehicle = vehicle,
            hub = hub
        };
        Actions.repairActions.Add(repairAction);
        onNewAction.Invoke(RepairActionInfo(repairAction));
        return repairAction;
    }

    public void RemoveRepairAction(string id)
    {
        Actions.repairActions =
            Actions
                .repairActions
                .Where(x => x.id != id)
                .ToList();
    }

    public QTransferAction AddTransferAction(
        string vehicle,
        string counterparty,
        QResourceTypeEnum resourceType,
        float quantity,
        QResourceTransferTypeEnum transferType)
    {
        var transferAction = new QTransferAction
        {
            id = $"{CurrentSimulation.id}:{vehicle}:{counterparty}",
            sim = CurrentSimulation.id,
            vehicle = vehicle,
            counterparty = counterparty,
            resourceType = resourceType,
            quantity = quantity,
            transferType = transferType
        };

        Actions.transferActions.Add(transferAction);
        onNewAction.Invoke(TransferActionInfo(transferAction));
        return transferAction;
    }

    public void RemoveTransferAction(string id)
    {
        Actions.transferActions =
            Actions
                .transferActions
                .Where(x => x.id != id)
                .ToList();
    }

    private static ActionInfo TransitActionInfo(QTransitAction action)
    {
        var vehicle = FormatEntityIdDisplay(action.vehicle);
        return new ActionInfo
        {
            ID = action.id,
            Type = ActionType.Transit,
            DisplayText = $"{vehicle} → ({action.destX}, {action.destY})"
        };
    }

    private static ActionInfo RepairActionInfo(QRepairAction action)
    {
        var vehicle = FormatEntityIdDisplay(action.vehicle);
        var hub = FormatEntityIdDisplay(action.hub);
        return new ActionInfo
        {
            ID = action.id,
            Type = ActionType.Repair,
            DisplayText = $"{vehicle} → {hub}"
        };
    }

    private static ActionInfo TransferActionInfo(QTransferAction action)
    {
        var vehicle = FormatEntityIdDisplay(action.vehicle);
        var counterparty = FormatEntityIdDisplay(action.counterparty);
        var resource = FormatEntityIdDisplay(action.resourceType.id);
        var dir = action.transferType.id == "Withdrawal" ? "←" : "→";
        return new ActionInfo
        {
            ID = action.id,
            Type = ActionType.Transfer,
            DisplayText = $"{vehicle} {dir} {counterparty}: {resource} x {action.quantity}"
        };
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

                actions.transitActions.ForEach(x => { onNewAction.Invoke(TransitActionInfo(x)); });
                actions.repairActions.ForEach(x => { onNewAction.Invoke(RepairActionInfo(x)); });
                actions.transferActions.ForEach(x => { onNewAction.Invoke(TransferActionInfo(x)); });

                callback.Invoke(actions);
            });
    }

    public void Simulate(Action<QState> callback)
    {
        Busy();

        IssueActions(Actions, success =>
        {
            print("Issue: " + Actions);
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

    public class ActionInfo
    {
        public string DisplayText;
        public string ID;
        public ActionType Type;
    }
}