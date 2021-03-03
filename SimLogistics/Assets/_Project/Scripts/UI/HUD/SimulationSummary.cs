using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimulationSummary : UIElement
{
    [SerializeField] private float initialShowDelay;

    [SerializeField] private TMP_Text simName;
    [SerializeField] private TMP_Text balance;
    [SerializeField] private TMP_Text income;
    [SerializeField] private TMP_Text expenses;
    
    private void Start()
    {
        simName.text = "---";
        balance.text = "---";
        income.text = "---";
        expenses.text = "---";
    }

    public void ShowFirstTime()
    {
        SetVisible(true, Effect.Animate).setDelay(initialShowDelay);
    }
    
    public void Show()
    {
        print("Simulation Summary Show");
        SetVisible(true, Effect.Animate);
    }
    
    public void Hide()
    {
        print("Simulation Summary Hide");
        SetVisible(false, Effect.Animate);
    }

}
