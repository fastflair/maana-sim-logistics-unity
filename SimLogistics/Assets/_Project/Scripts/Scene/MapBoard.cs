using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoard : MonoBehaviour
{
    private void Start()
    {
        var material = gameObject.GetComponent<Renderer>().material;
        var color = material.color;
        color.a = 0f;
        material.color = color;
    }

    public void FadeIn()
    {
        LeanTween.alpha(gameObject, 1f, 3f);
    }
}
