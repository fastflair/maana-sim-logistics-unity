using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSimulationDialog : Dialog
{
    [SerializeField] private InputField simName;
    [SerializeField] private InputField agentEndpoint;
    [SerializeField] private Button okayButton;
    
    private QSimulation _currentSimulation;

    private SimulationManager _simulationManager;
    public SimulationManager SimulationManager
    {
        get => _simulationManager;
        set
        {
            _simulationManager = value;
            _currentSimulation = _simulationManager.CurrentSimulation;

            simName.Value = _currentSimulation.name;
            agentEndpoint.Value = _currentSimulation.agentEndpoint;
            
            simName.Input.onValueChanged.AddListener(delegate { UpdateButtons(); });

            UpdateButtons();
        }
    }
    
    public void OnCancel()
    {
        Hide();
    }

    public void OnOkay()
    {
        SimulationManager.List(simName.Value, agentEndpoint.Value, existing =>
        {
            if (existing.Count != 0)
            {
                var dialog = UIManager.ShowConfirmationDialog($"A simulation with this name (and agent endpoint) already exists.{Environment.NewLine}Do you want to overwrite it?");
                dialog.okayEvent.AddListener(() =>
                {
                    SimulationManager.Overwrite(simName.Value, agentEndpoint.Value, simulation =>
                    {
                        Hide();
                    });
                });
                dialog.cancelEvent.AddListener(() =>
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
            simName.Value != null && 
            simName.Value != SimulationManager.DefaultSimulation.name;
    }
}
