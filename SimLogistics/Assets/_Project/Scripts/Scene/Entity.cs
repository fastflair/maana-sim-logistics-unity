using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;
    
    [HideInInspector]
    public UIManager uiManager;
    
    public QEntity QEntity;
    
    public void OnHoverEnter()
    {
        var card = uiManager.SpawnCard(cardPrefab, QEntity.id);
        if (card == null) return;
        
        card.Populate(card, QEntity);
        uiManager.ShowCard(card, QEntity.id);
    }
}
