using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public abstract class EntityManager<TQEntity> : MonoBehaviour 
    where TQEntity : QEntity
{
    [SerializeField] private UnityEvent onSpawned;

    [SerializeField] protected UIManager uiManager;
    [SerializeField] protected ConnectionManager connectionManager;
    [SerializeField] protected StringVariable simName;
    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable tileDropHeight;
    [SerializeField] private FloatVariable spawnDelay;

    protected List<TQEntity> QEntities { get; private set; }
    protected List<Entity> UEntities { get; private set; }

    protected abstract string QueryName { get; }
    protected abstract string Query { get; }
    protected abstract Entity EntityPrefab(TQEntity qEntity);
    
    public virtual void Spawn()
    {
        Destroy();
        QueryQ();
    }

    public virtual void Destroy()
    {
        print("OnDestroy: " + name);
        
        if (UEntities is null) return;
        
        foreach (var entity in UEntities)
        {
            print("Destroying: " + entity.name);
            Object.Destroy(entity.gameObject);
        }

        UEntities = null;
    }
    
    private void QueryQ()
    {
        connectionManager.QueryRaiseOnError<List<TQEntity>>(
            connectionManager.apiEndpoint,
            Query,
            QueryName,
            qEntities =>
            {
                Destroy();

                UEntities = new List<Entity>();

                QEntities = qEntities;

                StartCoroutine(Co_Spawn());
            });
    }
    
    private IEnumerator Co_Spawn()
    {
        foreach (var qEntity in QEntities)
        {
            var posX = tileSize.Value * qEntity.x;
            var posZ = -tileSize.Value * qEntity.y;

            var prefab = EntityPrefab(qEntity);
            var entity = Instantiate(prefab, new Vector3(posX, tileDropHeight.Value * tileSize.Value, posZ), Quaternion.identity);
            entity.uiManager = uiManager;
            entity.QEntity = qEntity;
            UEntities.Add(entity);
            
            yield return new WaitForSeconds(spawnDelay.Value);
        }

        onSpawned.Invoke();
    }
}
