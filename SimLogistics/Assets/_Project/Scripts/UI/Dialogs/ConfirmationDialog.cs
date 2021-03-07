using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConfirmationDialog : MessageDialog
{
    public UnityEvent onCancel = new UnityEvent();
    public UnityEvent onOkay = new UnityEvent();

    public void OnCancel()
    {
        onCancel.Invoke();
        Hide();
    }

    public override void OnOkay()
    {
        onOkay.Invoke();
        base.OnOkay();
    }
}