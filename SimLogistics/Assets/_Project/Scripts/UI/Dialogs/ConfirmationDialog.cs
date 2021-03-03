using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConfirmationDialog : MessageDialog
{
    [SerializeField] private UnityEvent<object> cancelEvent;
    [SerializeField] private UnityEvent<object> okayEvent;

    public object context { get; set; }
    
    public void OnCancel()
    {
        print("OnCancel");
        Hide();
        cancelEvent.Invoke(context);
    }

    public override void OnOkay()
    {
        print("OnOkay");
        base.OnOkay();
        okayEvent.Invoke(context);
    }
}