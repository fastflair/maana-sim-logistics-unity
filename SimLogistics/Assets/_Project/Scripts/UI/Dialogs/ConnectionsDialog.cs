using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionsDialog : Dialog
{
    [SerializeField] private InputField inputFieldPrefab;
    [SerializeField] private Transform fieldHost;
    [SerializeField] private Button buttonItemPrefab;
    [SerializeField] private Transform listHost;
    [SerializeField] private Button newButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button saveButton;
    
    public ConnectionManager ConnectionManager { get; set; }

    private readonly Dictionary<string, InputField> _fieldMap = new Dictionary<string, InputField>();

    private ConnectionState _currentState;
        
    private void Start()
    {
        GenerateForm();
        
        _currentState = ConnectionManager.CurrentConnectionState;
        if (_currentState == null) return;

        PopulateForm(_currentState);
    }

    private void GenerateForm()
    {
        AddField("Name");
        AddField("API Endpoint");
        AddField("Auth Domain");
        AddField("Auth Client ID");
        AddField("Auth Client Secret");
        AddField("Auth Identifier");
    }
    
    private void AddField(string label)
    {
        var inputField = Instantiate(inputFieldPrefab, fieldHost, false);
        inputField.Label = label;
        inputField.Input.onValueChanged.AddListener(delegate { UpdateButtons(); });
        _fieldMap.Add(label, inputField);
    }

    private void PopulateForm(ConnectionState state)
    {
        _fieldMap["Name"].Value = state.id;
        _fieldMap["API Endpoint"].Value = state.apiEndpoint;
        _fieldMap["Auth Domain"].Value = state.authDomain;
        _fieldMap["Auth Client ID"].Value = state.authClientId;
        _fieldMap["Auth Client Secret"].Value = state.authClientSecret;
        _fieldMap["Auth Identifier"].Value = state.authIdentifier;

        PopulateList();

        UpdateButtons();
    }

    private void ClearForm()
    {
        foreach (var field in _fieldMap.Values) field.Value = "";
        ClearList();
    }

    private void Reset()
    {
        _currentState = null;
        ClearForm();
        UpdateButtons();
    }
    
    public void OnNew()
    {
        Reset();
    }

    public void OnDelete()
    {
        if (!ConnectionManager.Delete(_fieldMap["Name"].Value)) return;        
        Reset();
    }

    public void OnSave()
    {
        var apiEndpoint = _fieldMap["API Endpoint"].Value;

        if (!ConnectionManager.IsValidUrl(apiEndpoint))
        {
            UIManager.ShowErrorDialog("Invalid URL for API Endpoint.");
            return;
        }
        
        _currentState = new ConnectionState
        {
            id = _fieldMap["Name"].Value,
            apiEndpoint = apiEndpoint,
            authDomain = _fieldMap["Auth Domain"].Value,
            authClientId = _fieldMap["Auth Client ID"].Value,
            authClientSecret = _fieldMap["Auth Client Secret"].Value,
            authIdentifier = _fieldMap["Auth Identifier"].Value
        };
        ConnectionManager.Save(_currentState);
        PopulateForm(_currentState);
    }

    public void OnCancel()
    {
        Hide();
    }
    
    public void OnDone()
    {
        if (_currentState != null)
        {
            ConnectionManager.LoadAndConnect(_currentState.id);
        }
        Hide();
    }
    
    private void UpdateButtons()
    {
        if (AreAnyFieldsPopulated())
        {
            var id = _fieldMap["Name"].Value;
            var isBootstrap = ConnectionManager.IsBoostrap(id);
            
            newButton.interactable = true;
            deleteButton.interactable = !isBootstrap;
            saveButton.interactable = !isBootstrap && AreAllFieldsPopulated();
        }
        else
        {
            newButton.interactable =
                deleteButton.interactable =
                    saveButton.interactable = false;
        }
    }

    private void PopulateList()
    {
        ClearList();
        
        foreach (var connection in ConnectionManager.AvailableConnections)
        {
            var buttonItem = Instantiate(buttonItemPrefab, listHost.transform, false);
            buttonItem.GetComponentInChildren<TMP_Text>().text = connection;
            buttonItem.onClick.AddListener(() =>
            {
                _currentState = ConnectionManager.Load(connection);
                PopulateForm(_currentState);
            });
        }
    }

    private void ClearList()
    {
        foreach (Transform buttonItem in listHost) Destroy(buttonItem.gameObject);
    }
    
    private bool IsCurrent(string id)
    {
        return id == ConnectionManager.CurrentConnectionState.id;
    }

    private bool AreAnyFieldsPopulated()
    {
        return _fieldMap.Values.Any(field => field.Value.Length > 0);
    }

    private bool AreAllFieldsPopulated()
    {
        return _fieldMap.Values.All(field => field.Value.Length > 0);
    }
}