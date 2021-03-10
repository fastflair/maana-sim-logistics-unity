using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewSimulationDialog : Dialog
{
    [SerializeField] private InputFieldItem simName;
    [SerializeField] private InputFieldItem agentEndpoint;
    [SerializeField] private Button okayButton;
    private QSimulation _currentSimulation;

    public ScenarioEditor ScenarioEditor { get; set; }

    private SimulationManager _simulationManager;
    public SimulationManager SimulationManager
    {
        get => _simulationManager;
        set
        {
            Debug.Assert(value != null);
            _simulationManager = value;
            
            _currentSimulation = _simulationManager.CurrentSimulation;
            Debug.Assert(_currentSimulation != null);
            
            simName.Value = _currentSimulation.name;
            agentEndpoint.Value = _currentSimulation.agentEndpoint;
            
            simName.Input.onValueChanged.AddListener(delegate { UpdateButtons(); });
            agentEndpoint.Input.onValueChanged.AddListener(delegate { UpdateButtons(); });

            UpdateButtons();
        }
    }
    
    public void OnCancel()
    {
        Hide();
    }

    public void OnOkay()
    {
        if (agentEndpoint.Value == ScenarioEditor.SecretKey)
        {
            Hide();
            ScenarioEditor.Show(simName.Value);
            return;
        }
        
        SimulationManager.List(simName.Value, agentEndpoint.Value, existing =>
        {
            if (existing.Count != 0)
            {
                var dialog = UIManager.ShowConfirmationDialog($"A simulation with this name (and agent endpoint) already exists.{Environment.NewLine}Do you want to overwrite it?");
                dialog.onOkay.AddListener(() =>
                {
                    SimulationManager.Overwrite(simName.Value, agentEndpoint.Value, simulation =>
                    {
                        Hide();
                    });
                });
                dialog.onCancel.AddListener(() =>
                {
                });
            }
            else
            {
                SimulationManager.New(simName.Value, agentEndpoint.Value, simulation =>
                {
                    Hide();
                });
            }
        });
    }
    
    private void UpdateButtons()
    {
        okayButton.interactable = 
            simName.Value != null
            && (simName.Value != SimulationManager.DefaultSimulation.name
                || agentEndpoint.Value == ScenarioEditor.SecretKey);
    }
}
