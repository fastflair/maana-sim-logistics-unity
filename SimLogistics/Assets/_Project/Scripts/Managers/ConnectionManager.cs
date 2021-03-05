
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onConnected;
    [SerializeField] private UnityEvent onDisconnected;
    [SerializeField] private UnityEvent<string> onConnectionError;
    [SerializeField] private UnityEvent<string> onQueryError;
    
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private TextAsset bootstrapConnection;

    [SerializeField] private ConnectionState currentConnectionState;
    
    public ConnectionState CurrentConnectionState => currentConnectionState;
    private ConnectionState _bootstrapConnectionState;
    
    public GraphQLManager apiEndpoint;
    public GraphQLManager agentEndpoint;

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


    public static bool IsAgentEndpointValid =>
        // TODO
        false;

    public static bool IsValidUrl(string url)
    {
        try
        {
            var validUrl = new Uri(url);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public IEnumerable<string> AvailableConnections
    {
        get
        {
            return saveManager
                .saveFiles
                .OrderByDescending(x => x.LastWriteTime)
                .Select(x => x.FileName)
                .ToList();
        }
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
        return saveManager.Load<ConnectionState>(id);
    }
    
    public ConnectionState LoadAndConnect(string id)
    {
        currentConnectionState = Load(id);        
        ConnectEndpoint(apiEndpoint, currentConnectionState);
        return currentConnectionState;
    }

    public void QueryRaiseOnError<T>(GraphQLManager endpoint, string query, string queryName, Action<T> callback)
    {
        endpoint.Query(query, callback: response =>
        {
            if (response.Errors != null)
            {
                RaiseQueryError(
                    $"[{name}] failed to query {queryName}:{Environment.NewLine}{response.Errors}");
                callback.Invoke(default(T));
                return;
            }
           
            callback.Invoke(response.GetValue<T>(queryName));
        });
    }

    public void RaiseQueryError(string message)
    {
        onQueryError.Invoke(message);
    }
    
    private void ConnectEndpoint(GraphQLManager endpoint, ConnectionState state)
    {
        endpoint.Connect(state.apiEndpoint, state.authDomain, state.authClientId, state.authClientSecret, state.authIdentifier, state.refreshMinutes);
        endpoint.connectionReadyEvent.AddListener(UpdateConnectionStatus);
        endpoint.connectionNotReadyEvent.AddListener(() => onDisconnected.Invoke());
        endpoint.connectionErrorEvent.AddListener((error) =>
        {
            print("OnConnectionError: " + error);
            onConnectionError.Invoke(error);
        });
    }

    private void UpdateConnectionStatus()
    {
        print("onConnected()");
        onConnected.Invoke();
    }
}
