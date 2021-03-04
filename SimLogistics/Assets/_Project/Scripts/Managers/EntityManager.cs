using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public abstract class EntityManager<TQEntity, TUEntity> : MonoBehaviour 
    where TQEntity : QEntity
    where TUEntity : Entity
{
    [SerializeField] private UnityEvent onSpawned;

    [SerializeField] protected ConnectionManager connectionManager;
    [SerializeField] protected StringVariable simName;
    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable tileDropHeight;
    [SerializeField] private FloatVariable spawnDelay;

    protected List<TQEntity> QEntities { get; private set; }
    protected List<TUEntity> UEntities { get; private set; }

    protected abstract string QueryName { get; }
    protected abstract string Query { get; }
    protected abstract TUEntity EntityPrefab(TQEntity qEntity);
    
    public virtual void Spawn()
    {
        Destroy();
        QueryQ();
    }

    public virtual void Destroy()
    {
        if (UEntities is null) return;
        
        foreach (var entity in UEntities)
        {
            Object.Destroy(entity);
        }    
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

                UEntities = new List<TUEntity>();

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
            var uEntity = Instantiate(prefab, new Vector3(posX, tileDropHeight.Value * tileSize.Value, posZ), Quaternion.identity);
            // var selectableObject = entityGameObject.AddComponent<SelectableObject>();
            // selectableObject.SelectionHandler = this;
            // selectableObject.id = qMapEntity.id;
            UEntities.Add(uEntity);
            
            yield return new WaitForSeconds(spawnDelay.Value);
        }

        onSpawned.Invoke();
    }

    // public void OnHoverEnter(GameObject obj)
    // {
    //     print($"OnHoverEnter: {obj.name}");
    // }
    //
    // public void OnHoverExit(GameObject obj)
    // {
    //     print($"OnHoverExit: {obj.name}");
    // }
    //
    // public void OnSelect(GameObject obj)
    // {
    //     print($"OnSelect: {obj.name}");
    // }
}
