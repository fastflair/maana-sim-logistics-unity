using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationDialog : MessageDialog
{
    public void OnCancel()
    {
        Hide();
    }

    public void OnOkay()
    {
        Hide();
    }
}