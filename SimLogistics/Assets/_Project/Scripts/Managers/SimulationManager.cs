using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] private string cityEndpoint = "maana-sim-logistics-city-v3";
    [SerializeField] private string producerEndpoint = "maana-sim-logistics-producer-v3";
    [SerializeField] private string hubEndpoint = "maana-sim-logistics-hub-v3";
    [SerializeField] private string vehicleEndpoint = "maana-sim-logistics-vehicle-v3";
    
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

    public static string FormatTransferDetailDisplay(QResourceTransfer transfer)
    {
        var res = transfer.resourceType.id;
        var dir = transfer.transferType.id == "Deposit" ? "→" : "←";
        var tot = transfer.quantity * transfer.price;
        return $"{dir} {transfer.quantity} x {res} @ {transfer.price}";
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

    public static int TransferComparer(QResourceTransfer x, QResourceTransfer y)
    {
        switch (x)
        {
            case null when y == null:
                return 0;
            case null:
                return -1;
        }

        if (y == null) return 1;
        if (x.status.id == y.status.id) return 0;
        if (x.status.id == "OK") return -1;
        return 1;
    }

    public static int TransitComparer(QTransitOrder x, QTransitOrder y)
    {
        switch (x)
        {
            case null when y == null:
                return 0;
            case null:
                return 1;
        }

        if (y == null) return -1;
        if (x.status.id == y.status.id) return 0;

        if (x.status.id == "Disabled") return -1;
        if (y.status.id == "Disabled") return 1;
        
        if (x.status.id != "Idle") return -1;
        if (y.status.id != "Idle") return 1;
        
        return 1;
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

        InternalStateQueryWithLoading(queryName, query, state =>
        {
            if (state != null) CurrentState = state;
            callback(state);
        });
    }

    public void Save(QState state, Action<QState> callback)
    {
        const string queryName = "saveState";
        var query = @$"
          {QStateFragment.withIncludes}
          mutation {{
            {queryName}(state: {state}) {{
              ...stateData
            }}
          }}
        ";

        InternalStateQuery(queryName, query, callback);
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

    public void Think(string agentEndpoint, QState state, Action<QActions> callback)
    {
        Busy();

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

        const string queryName = "simulate";
        var query = @$"
          {QStateFragment.withIncludes}
          query {{
            {queryName}(state: {CurrentState}, actions: {Actions}) {{
              ...stateData
            }}
          }}
        ";

        InternalStateQuery(queryName, query, state =>
        {
            NotBusy();
            if (state == null) return;

            ResetActions();
            
            CurrentState = state;
            onUpdated.Invoke();

            Save(state, _ => { });
        });
    }
    
    // --- Editor

    public void LoadCities(string sim, Action<List<QCity>> callback)
    {
        const string queryName = "selectCity";
        var query = @$"
          {QCityFragment.OutputDataWithIncludes}
          query {{
            {queryName}(sim: ""{sim}"") {{
              ...cityData
            }}
          }}
        ";
        connectionManager.QueryRaiseOnError(
            cityEndpoint,
            query,
            queryName,
            callback);
    }

    public void SaveCity(QCity city, Action<string> callback)
    {
        const string queryName = "addCity";
        var query = @$"
          mutation {{
            {queryName}(input: {{
              id: ""{city.id}""
              sim: ""{city.sim}""
              x: {city.x}
              y: {city.y}
              steps: {city.steps}
              population: {city.population}
              populationGrowthRate: {city.populationGrowthRate}
              populationDeclineRate: {city.populationDeclineRate}
              demand: [{string.Join(",",city.demand.Select(x => $"\"{x.id}\""))}]
           }})
          }}
        ";
        connectionManager.QueryRaiseOnError(
            cityEndpoint,
            query,
            queryName,
            callback);
    }
    
    public void DeleteCity(string id, Action<QId> callback)
    {
        const string queryName = "deleteCity";
        var query = @$"
          mutation {{
            {queryName}(id: ""{id}"") {{
              id
            }}
          }}
        ";
        connectionManager.QueryRaiseOnError(
            cityEndpoint,
            query,
            queryName,
            callback);
    }
    
    public void LoadProducers(string sim, Action<List<QProducer>> callback)
    {
        const string queryName = "selectProducer";
        var query = @$"
          {QProducerFragment.withIncludes}
          query {{
            {queryName}(sim: ""{sim}"") {{
              ...producerData
            }}
          }}
        ";
        connectionManager.QueryRaiseOnError(
            connectionManager.ApiEndpoint,
            query,
            queryName,
            callback);
    }
    public void LoadHubs(string sim, Action<List<QHub>> callback)
    {
        const string queryName = "selectHub";
        var query = @$"
          {QHubFragment.withIncludes}
          query {{
            {queryName}(sim: ""{sim}"") {{
              ...hubData
            }}
          }}
        ";
        connectionManager.QueryRaiseOnError(
            connectionManager.ApiEndpoint,
            query,
            queryName,
            callback);
    }
    public void LoadVehicles(string sim, Action<List<QVehicle>> callback)
    {
        const string queryName = "selectVehicle";
        var query = @$"
          {QVehicleFragment.withIncludes}
          query {{
            {queryName}(sim: ""{sim}"") {{
              ...vehicleData
            }}
          }}
        ";
        connectionManager.QueryRaiseOnError(
            connectionManager.ApiEndpoint,
            query,
            queryName,
            callback);
    }

    // --- Internal

    private void LoadDefault()
    {
        Load(DefaultSimulation.id, _ => { });
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

        InternalStateQueryWithLoading(queryName, query, state =>
        {
            if (state != null) CurrentState = state;
            callback(state);
        });
    }

    private void InternalStateQueryWithLoading(string queryName, string query, Action<QState> callback)
    {
        onLoading.Invoke();
        InternalStateQuery(
            queryName,
            query,
            state =>
            {
                if (state != null) CurrentState = state;
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
        connectionManager.QueryRaiseOnError(
            connectionManager.ApiEndpoint,
            query,
            queryName,
            callback);
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