using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionsDialog : Dialog
{
    [SerializeField] private Button buttonItemPrefab;
    public ConnectionManager ConnectionManager { get; set; }

    private readonly Dictionary<string, TMP_InputField> _fieldMap = new Dictionary<string, TMP_InputField>();

    private Transform _listContent;

    private Button _newButton;
    private Button _deleteButton;
    private Button _saveButton;

    private void Start()
    {
        _listContent = transform.Find("Box/Body/Connection List/Viewport/Content");

        var fieldGroup = transform.Find("Box/Body/Content");
        for (var i = 0; i < fieldGroup.childCount; i++)
        {
            var child = fieldGroup.GetChild(i);
            var input = GetInputField(child);
            input.onValueChanged.AddListener(delegate { UpdateButtons(); });
            _fieldMap.Add(child.name, input);
        }

        var buttonBar = transform.Find("Box/Body/Command Bar");
        _newButton = buttonBar.Find("New").GetComponent<Button>();
        _deleteButton = buttonBar.Find("Delete").GetComponent<Button>();
        _saveButton = buttonBar.Find("Save").GetComponent<Button>();

        PopulateForm();
    }

    public void PopulateForm()
    {
        var state = ConnectionManager.CurrentConnectionState;
        if (state == null) return;

        PopulateFormWithState(state);
    }

    public void PopulateFormWithState(ConnectionState state)
    {
        _fieldMap["Name"].text = state.id;
        _fieldMap["API Endpoint"].text = state.apiEndpoint;
        _fieldMap["Agent Endpoint"].text = state.agentEndpoint;
        _fieldMap["Auth Domain"].text = state.authDomain;
        _fieldMap["Auth Client ID"].text = state.authClientId;
        _fieldMap["Auth Client Secret"].text = state.authClientSecret;
        _fieldMap["Auth Identifier"].text = state.authIdentifier;

        PopulateList();

        UpdateButtons();
    }

    public void ClearForm()
    {
        foreach (var field in _fieldMap.Values) field.text = "";
    }
    
    public void OnNew()
    {
        ClearForm();
        UpdateButtons();
    }

    public void OnDelete()
    {
        if (!ConnectionManager.Delete(_fieldMap["Name"].text)) return;
        
        ClearForm();
        PopulateForm();
    }

    public void OnSave()
    {
        var state = new ConnectionState
        {
            id = _fieldMap["Name"].text,
            apiEndpoint = _fieldMap["API Endpoint"].text,
            agentEndpoint = _fieldMap["Agent Endpoint"].text,
            authDomain = _fieldMap["Auth Domain"].text,
            authClientId = _fieldMap["Auth Client ID"].text,
            authClientSecret = _fieldMap["Auth Client Secret"].text,
            authIdentifier = _fieldMap["Auth Identifier"].text
        };
        ConnectionManager.Save(state);
        PopulateForm();
    }

    public void OnDone()
    {
        Hide();
    }

    private static TMP_InputField GetInputField(Transform t)
    {
        return t.Find("Input").GetComponent<TMP_InputField>();
    }

    private void UpdateButtons()
    {
        if (AreAnyFieldsPopulated())
        {
            var id = _fieldMap["Name"].text;
            
            _newButton.interactable = true;
            _deleteButton.interactable = IsCurrent(id) && !ConnectionManager.IsBoostrap(id);
            _saveButton.interactable = AreAllFieldsPopulated();
        }
        else
        {
            _newButton.interactable =
                _deleteButton.interactable =
                    _saveButton.interactable = false;
        }
    }

    private void PopulateList()
    {
        foreach (Transform buttonItem in _listContent) Destroy(buttonItem.gameObject);

        foreach (var connection in ConnectionManager.AvailableConnections)
        {
            var buttonItem = Instantiate(buttonItemPrefab, _listContent.transform, false);
            buttonItem.GetComponentInChildren<TMP_Text>().text = connection;
            buttonItem.onClick.AddListener(() => PopulateFormWithState(ConnectionManager.LoadAndConnect(connection)));
        }
    }

    private bool IsCurrent(string id)
    {
        return id == ConnectionManager.CurrentConnectionState.id;
    }

    private bool AreAnyFieldsPopulated()
    {
        return _fieldMap.Values.Any(field => field.text.Length > 0);
    }

    private bool AreAllFieldsPopulated()
    {
        return _fieldMap.Values.All(field => field.text.Length > 0);
    }
}