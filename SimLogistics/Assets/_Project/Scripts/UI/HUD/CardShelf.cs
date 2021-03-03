using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardShelf : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        print("Card Shelf Show");
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        print("Card Shelf Hide");
        gameObject.SetActive(false);
    }
}
