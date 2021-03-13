// using System;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class EntityManager<TQEntity> : MonoBehaviour
    where TQEntity : QEntity
{
    [SerializeField] private UnityEvent onSpawned;

    [SerializeField] protected CardHost cardHost;
    [SerializeField] protected MapManager mapManager;
    [SerializeField] protected SimulationManager simulationManager;
    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable entityOffsetX;
    [SerializeField] private FloatVariable entityOffsetY;
    [SerializeField] private FloatVariable spawnHeight;
    [SerializeField] private FloatVariable spawnDelay;
    [SerializeField] private float lerpSpeed = 3;

    protected List<Entity> UEntities = new List<Entity>();

    private float EntityXToWorldX(float x) => tileSize.Value * x + entityOffsetX.Value;
    private float EntityYToWorldZ(float y) => -(tileSize.Value * y + entityOffsetY.Value);


    // Interface
    protected abstract IEnumerable<TQEntity> QEntities { get; }
    public abstract Entity EntityPrefab(TQEntity qEntity);

    public virtual void Spawn()
    {
        Destroy();
        StartCoroutine(Co_Spawn());
    }

    public virtual void Destroy()
    {
        if (UEntities is null) return;

        foreach (var entity in UEntities) Destroy(entity.gameObject);

        UEntities = new List<Entity>();
    }

    public void OnUpdate()
    {
        foreach (var entity in UEntities)
        {
            entity.QEntity = QEntities.First(qEntity => qEntity.id == entity.QEntity.id);
            var entityTransform = entity.transform;
            var newPosX = EntityXToWorldX(entity.QEntity.x);
            var newPosZ = EntityYToWorldZ(entity.QEntity.y);
            if (!(Math.Abs(newPosX - entityTransform.position.x) > float.Epsilon) &&
                !(Math.Abs(newPosZ - entityTransform.position.z) > float.Epsilon))
            {
                // print($"[{name}] {SimulationManager.FormatEntityIdDisplay(entity.QEntity.id)} - no change.");
                continue;
            }

            // print($"[{name}] {SimulationManager.FormatEntityIdDisplay(entity.QEntity.id)} -> ({newPosX}, {newPosZ})");

            var newPosition = new Vector3(newPosX, entityTransform.position.y, newPosZ);
            StartCoroutine(LerpPosition(entityTransform, newPosition, lerpSpeed));
        }
    }

    private static IEnumerator LerpPosition(Transform entityTransform, Vector3 targetPosition, float duration)
    {
        float time = 0;
        var startPosition = entityTransform.position;

        while (time < duration)
        {
            entityTransform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        entityTransform.position = targetPosition;
    }

    private IEnumerator Co_Spawn()
    {
        foreach (var qEntity in QEntities)
        {
            // print($"Spawning entity: {qEntity}");

            var prefab = EntityPrefab(qEntity);
            var quaternion = Helpers.QuaternionFromEuler(0f, qEntity.yRot, 0f);

            var entity = Instantiate(prefab,
                new Vector3(EntityXToWorldX(qEntity.x), spawnHeight.Value * tileSize.Value, EntityYToWorldZ(qEntity.y)),
                quaternion);
            entity.cardHost = cardHost;
            entity.QEntity = qEntity;
            UEntities.Add(entity);

            yield return new WaitForSeconds(spawnDelay.Value);
        }

        onSpawned.Invoke();
    }
}