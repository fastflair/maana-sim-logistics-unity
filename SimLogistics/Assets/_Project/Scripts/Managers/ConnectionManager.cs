
using System.Security.Cryptography.X509Certificates;
using Maana.GraphQL;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private ConnectionState currentConnectionState;
    [SerializeField] private TextAsset bootstrapConnection;

    public GraphQLManager apiEndpoint;
    public GraphQLManager agentEndpoint;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (saveManager.CurrentSaveFile == null)
        {
            print("Bootstrapping connection");
            currentConnectionState = JsonUtility.FromJson<ConnectionState>(bootstrapConnection.text);
            saveManager.Save(currentConnectionState.id, currentConnectionState);
        }
        
        LoadAndConnect(saveManager.CurrentSaveFile.FileName);
    }

    public void LoadAndConnect(string fileName)
    {
        print("Loading connection");
        currentConnectionState = saveManager.Load<ConnectionState>(fileName);
        print("Connection: " + currentConnectionState.id);
        
        ConnectEndpoint(apiEndpoint, currentConnectionState);
        ConnectEndpoint(agentEndpoint, currentConnectionState);
    }
    
    private void ConnectEndpoint(GraphQLManager endpoint, ConnectionState state)
    {
        var url = endpoint == apiEndpoint ? state.apiEndpoint : state.agentEndpoint;
        print("Connecting to: " + url);
        endpoint.Connect(url, state.authDomain, state.authClientId, state.authClientSecret, state.authIdentifier, state.refreshMinutes);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
