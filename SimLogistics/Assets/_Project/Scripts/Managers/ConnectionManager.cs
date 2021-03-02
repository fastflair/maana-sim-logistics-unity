
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
    
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private TextAsset bootstrapConnection;

    [SerializeField] private ConnectionState currentConnectionState;
    
    public ConnectionState CurrentConnectionState => currentConnectionState;
    private ConnectionState _bootstrapConnectionState;
    
    public GraphQLManager apiEndpoint;
    public GraphQLManager agentEndpoint;

    private int _connectionCount;
    
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
    
    public IEnumerable<string> AvailableConnections
    {
        get { return saveManager.saveFiles.Select((x) => x.FileName).ToList(); }
    }

    public bool Save(ConnectionState state)
    {
        if (!saveManager.Save(state.id, state)) return false;
        Reload();
        return true;

    }

    public bool Delete(string id)
    {
        if (id == _bootstrapConnectionState.id) return false;
        saveManager.Delete(id);
        Reload();        
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
        ConnectEndpoint(agentEndpoint, currentConnectionState);

        return currentConnectionState;
    }
    
    private void ConnectEndpoint(GraphQLManager endpoint, ConnectionState state)
    {
        var url = endpoint == apiEndpoint ? state.apiEndpoint : state.agentEndpoint;
        endpoint.Connect(url, state.authDomain, state.authClientId, state.authClientSecret, state.authIdentifier, state.refreshMinutes);
        endpoint.onConnected.AddListener(UpdateConnectionStatus);
        endpoint.onDisconnected.AddListener(() => onDisconnected.Invoke());
        endpoint.onConnectionError.AddListener((error) => onConnectionError.Invoke(error));
    }

    private void UpdateConnectionStatus()
    {
        if (_connectionCount++ % 2 == 0)
        {
            onConnected.Invoke();
        }
    }
}
