using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EntityManager<TQEntity> : MonoBehaviour
    where TQEntity : QEntity
{
    [SerializeField] private UnityEvent onSpawned;

    [SerializeField] protected UIManager uiManager;
    [SerializeField] protected SimulationManager simulationManager;

    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable spawnHeight;
    [SerializeField] private FloatVariable spawnDelay;

    private List<Entity> _uEntities = new List<Entity>();

    // Interface
    protected abstract IEnumerable<TQEntity> QEntities { get; }
    protected abstract Entity EntityPrefab(TQEntity qEntity);

    public virtual void Spawn()
    {
        Destroy();
        StartCoroutine(Co_Spawn());
    }

    public virtual void Destroy()
    {
        if (_uEntities is null) return;

        foreach (var entity in _uEntities) Destroy(entity.gameObject);

        _uEntities = new List<Entity>();
    }

    private IEnumerator Co_Spawn()
    {
        foreach (var qEntity in QEntities)
        {
            var posX = tileSize.Value * qEntity.x;
            var posZ = -tileSize.Value * qEntity.y;

            var prefab = EntityPrefab(qEntity);
            var entity = Instantiate(prefab, new Vector3(posX, spawnHeight.Value * tileSize.Value, posZ),
                Quaternion.identity);
            entity.uiManager = uiManager;
            entity.QEntity = qEntity;
            _uEntities.Add(entity);

            yield return new WaitForSeconds(spawnDelay.Value);
        }

        onSpawned.Invoke();
    }
}