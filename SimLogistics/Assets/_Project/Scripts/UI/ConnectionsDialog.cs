using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionsDialog : MonoBehaviour
{
    [SerializeField] private ConnectionManager connectionManager;

    private Button _newButton;
    private Button _deleteButton;
    private Button _saveButton;
    
    private Dictionary<string, TMP_InputField> _fieldMap = new Dictionary<string, TMP_InputField>();
    
    private void Start()
    {
        var fieldGroup = transform.Find("Box/Body/Field Group");
        for (var i = 0; i < fieldGroup.childCount; i++)
        {
            var child = fieldGroup.GetChild(i);
            var input = GetInputField(child);
            _fieldMap.Add(child.name, input);
        }

        var buttonBar = transform.Find("Box/Body/Command Bar");
        _newButton = buttonBar.Find("New").GetComponent<Button>();
        _deleteButton = buttonBar.Find("Delete").GetComponent<Button>();
        _saveButton = buttonBar.Find("Save").GetComponent<Button>();

        UpdateButtons();
    }
    
    public void PopulateForm()
    {
        var state = connectionManager.CurrentConnectionState;
        if (state == null) return;
        
        _fieldMap["Name"].text = state.id;
        _fieldMap["API Endpoint"].text = state.apiEndpoint;
        _fieldMap["Agent Endpoint"].text = state.agentEndpoint;
        _fieldMap["Auth Domain"].text = state.authDomain;
        _fieldMap["Auth Client ID"].text = state.authClientId;
        _fieldMap["Auth Client Secret"].text = state.authClientSecret;
        _fieldMap["Auth Identifier"].text = state.authIdentifier;

        UpdateButtons();
    }

    private static TMP_InputField GetInputField(Transform t)
    {
        return t.Find("Input").GetComponent<TMP_InputField>();
    }

    private void UpdateButtons()
    {
        if (AreAnyFieldsPopulated())
        {
            _newButton.interactable = true;
            _deleteButton.interactable = IsCurrent();
            _saveButton.interactable = AreAllFieldsPopulated();
        }
        else
        {
            _newButton.interactable = 
            _deleteButton.interactable = 
            _saveButton.interactable = false;
        }
    }

    private bool IsCurrent()
    {
        var name = _fieldMap["Name"].text;
        var id = connectionManager.CurrentConnectionState.id;
        print("Name: " + name + ", id: " + id);
        return name == id;
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
