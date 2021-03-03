using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConfirmationDialog : MessageDialog
{
    public UnityEvent cancelEvent = new UnityEvent();
    public UnityEvent okayEvent = new UnityEvent();

    public void OnCancel()
    {
        print("OnCancel");
        cancelEvent.Invoke();
        Hide();
    }

    public override void OnOkay()
    {
        print("OnOkay");
        okayEvent.Invoke();
        base.OnOkay();
    }
}