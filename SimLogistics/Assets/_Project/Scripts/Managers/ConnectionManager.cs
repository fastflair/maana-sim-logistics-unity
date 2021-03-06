using System;
using System.Collections.Generic;
using System.Linq;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onConnected;
    [SerializeField] private UnityEvent onDisconnected;
    [SerializeField] private UnityEvent<string> onConnectionError;
    [SerializeField] private UnityEvent<string> onQueryError;

    [SerializeField] private GraphQLManager server;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private TextAsset bootstrapConnection;
    private ConnectionState _bootstrapConnectionState;

    public ConnectionState CurrentConnectionState { get; private set; }
    public string ApiEndpoint => CurrentConnectionState.apiEndpoint;

    public GraphQLManager Server => server;

    public IEnumerable<string> AvailableConnections =>
        saveManager
            .saveFiles
            .OrderByDescending(x => x.LastWriteTime)
            .Select(x => x.FileName)
            .ToList();

    private void Start()
    {
        _bootstrapConnectionState = JsonUtility.FromJson<ConnectionState>(bootstrapConnection.text);
        Reload();
    }

    private void Reload()
    {
        string id;
        if (saveManager.CurrentSaveFile != null)
        {
            id = saveManager.CurrentSaveFile.FileName;
        }
        else
        {
            // Bootstrap
            Save(_bootstrapConnectionState);
            id = _bootstrapConnectionState.id;
        }

        LoadAndConnect(id);
    }

    public bool IsBoostrap(string id)
    {
        return id == _bootstrapConnectionState.id;
    }

    public bool Rebootstrap()
    {
        return saveManager.Save(_bootstrapConnectionState.id, _bootstrapConnectionState);
    }

    public bool Save(ConnectionState state)
    {
        return saveManager.Save(state.id, state);
    }

    public bool Delete(string id)
    {
        if (id == _bootstrapConnectionState.id) return false;
        saveManager.Delete(id);
        return true;
    }

    public ConnectionState Load(string id)
    {
        try
        {
            return saveManager.Load<ConnectionState>(id);
        }
        catch
        {
            saveManager.Delete(id);
            return saveManager.Load<ConnectionState>(_bootstrapConnectionState.id);
        }
    }

    public ConnectionState LoadAndConnect(string id)
    {
        CurrentConnectionState = Load(id);
        if (CurrentConnectionState.url == null)
        {
            Rebootstrap();
            CurrentConnectionState = _bootstrapConnectionState;
        }
        ConnectEndpoint(CurrentConnectionState);
        return CurrentConnectionState;
    }

    public void QueryRaiseOnError<T>(string endpoint, string query, string queryName, Action<T> callback)
    {
        server.Query(endpoint, query, callback: response =>
        {
            if (response.Errors != null)
            {
                RaiseQueryError(
                    $"[{name}] failed to query {queryName}:{Environment.NewLine}{response.Errors}");
                callback.Invoke(default);
                return;
            }

            print($"response: {response.Raw}");

            callback.Invoke(response.GetValue<T>(queryName));
        });
    }

    public void RaiseQueryError(string message)
    {
        onQueryError.Invoke(message);
    }

    private void ConnectEndpoint(ConnectionState state)
    {
        print($"Connecting to {state.url}");

        server.Connect(
            state.url,
            state.authDomain,
            state.authClientId,
            state.authClientSecret,
            state.authIdentifier,
            state.refreshMinutes);
        server.connectionReadyEvent.AddListener(UpdateConnectionStatus);
        server.connectionNotReadyEvent.AddListener(() => onDisconnected.Invoke());
        server.connectionErrorEvent.AddListener(error =>
        {
            onConnectionError.Invoke(error);

            if (CurrentConnectionState.id == _bootstrapConnectionState.id) return;

            Rebootstrap();
            ConnectEndpoint(CurrentConnectionState);
        });
    }

    private void UpdateConnectionStatus()
    {
        onConnected.Invoke();
    }
}