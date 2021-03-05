using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class SimulationSummary : UIElement
{
    [SerializeField] private TMP_Text simName;
    [SerializeField] private TMP_Text steps;
    [SerializeField] private TMP_Text balance;
    [SerializeField] private TMP_Text income;
    [SerializeField] private TMP_Text expenses;

    [SerializeField] private SimulationManager simulationManager;
 
    public void Show()
    {
        UpdateSummary();
        SetVisible(true);
    }

    public void ShowAnimate()
    {
        UpdateSummary();
        SetVisible(true, Effect.Animate); 
    }

    public void Hide()
    {
        SetVisible(false);
    }

    public void HideAnimate()
    {
        SetVisible(false, Effect.Animate);
    }

    public void UpdateSummary()
    {
        var sim = simulationManager.CurrentSimulation;
        simName.text = sim.name;
        steps.text = sim.steps.ToString();
        balance.text = sim.balance.ToString(CultureInfo.CurrentCulture);
        income.text = sim.income.ToString(CultureInfo.CurrentCulture);
        expenses.text = sim.expenses.ToString(CultureInfo.CurrentCulture);
    }
}
