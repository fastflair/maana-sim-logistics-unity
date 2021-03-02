using UnityEngine;

public class Dialog : MonoBehaviour
{
    public UIManager UIManager { get; set; }

    protected void Show()
    {
        UIManager.ShowDialog(this);
    }
    
    protected void Hide()
    {
        UIManager.HideDialog(this);
    }
}