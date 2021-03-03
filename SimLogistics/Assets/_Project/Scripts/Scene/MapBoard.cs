using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoard : MonoBehaviour
{
    [SerializeField] private float fadeDuration;
    [SerializeField] private float fadeDelay;
    
    private void Start()
    {
        // var material = gameObject.GetComponent<Renderer>().material;
        // var color = material.color;
        // color.a = 0f;
        // material.color = color;
    }

    public void FadeIn()
    {
        print("MapBoard: FadeIn " + gameObject.activeSelf);
        
        // LeanTween.alpha(gameObject, 1f, fadeDuration).setDelay(fadeDelay);
    }
}
