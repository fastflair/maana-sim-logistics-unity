using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class SimulationSummary : UIElement
{
    [SerializeField] private float initialShowDelay;
    private bool _hasShown;

    [SerializeField] private TMP_Text simName;
    [SerializeField] private TMP_Text steps;
    [SerializeField] private TMP_Text balance;
    [SerializeField] private TMP_Text income;
    [SerializeField] private TMP_Text expenses;

    [SerializeField] private SimulationManager simulationManager;
    
    public void ShowFirstTime()
    {
        print($"[{name}] ShowFirstTime: _hasShown={_hasShown}");
        if (_hasShown) return;
        _hasShown = true;
        SetVisible(true, Effect.Animate).setDelay(initialShowDelay);
    }
    
    public void Show()
    {
        if (!_hasShown) return;
        
        print($"[{name}] Show");
        SetVisible(true, Effect.Animate);
    }
    
    public void Hide()
    {
        print($"[{name}] Hide");
        SetVisible(false, Effect.Animate);
    }

    public void UpdateSummary()
    {
        print($"[{name}] UpdateSummary");
        var sim = simulationManager.CurrentSimulation;
        print("sim: " + sim.name);
        simName.text = sim.name;
        steps.text = sim.steps.ToString();
        balance.text = sim.balance.ToString(CultureInfo.CurrentCulture);
        income.text = sim.income.ToString(CultureInfo.CurrentCulture);
        expenses.text = sim.expenses.ToString(CultureInfo.CurrentCulture);
    }
}
