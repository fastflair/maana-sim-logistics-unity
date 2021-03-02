using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationDialog : Dialog
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