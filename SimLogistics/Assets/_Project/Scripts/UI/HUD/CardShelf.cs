using UnityEngine;

public class CardShelf : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        print($"[{name}] Show");
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        print($"[{name}] Hide");
        gameObject.SetActive(false);
    }
}
