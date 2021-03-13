using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public enum EntityType
    {
        City,
        Producer,
        Hub,
        Vehicle
    }
    
    [SerializeField] private EntityType type;

    [HideInInspector] public CardHost cardHost;

    public QEntity QEntity;

    public void OnHoverEnter()
    {
        // cardHost.ShowCardForEntity(type, QEntity);
    }

    public void OnSelect()
    {
        cardHost.ShowCardForEntity(type, QEntity);
    }
}