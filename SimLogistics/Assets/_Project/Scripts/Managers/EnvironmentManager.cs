using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private GameObject clouds;

    public void Activate()
    {
        clouds.SetActive(true);   
    }

    public void Deactivate()
    {
        clouds.SetActive(false);
    }
}
