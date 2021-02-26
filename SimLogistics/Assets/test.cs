using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class test : MonoBehaviour
{
        [SerializeField] private UnityEvent onTestSpawned;

        private void Start()
        {
                onTestSpawned.Invoke();
        }
}
