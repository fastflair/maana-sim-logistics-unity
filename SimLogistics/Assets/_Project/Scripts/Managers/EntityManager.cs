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
    [SerializeField] protected SelectionManager selectionManager;
    [SerializeField] protected MapManager mapManager;
    [SerializeField] protected SimulationManager simulationManager;
    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable entityOffsetX;
    [SerializeField] private FloatVariable entityOffsetY;
    [SerializeField] private FloatVariable spawnHeight;
    [SerializeField] private FloatVariable spawnDelay;

    public List<Entity> UEntities { get; set; } = new List<Entity>();

    public float EntityXToWorldX(float x) => tileSize.Value * x + entityOffsetX.Value;
    public float EntityYToWorldZ(float y) => -(tileSize.Value * y + entityOffsetY.Value);


    // Interface
    public abstract IEnumerable<TQEntity> QEntities { get; }
    public abstract Entity EntityPrefab(TQEntity qEntity);

    public Entity Entity(string id)
    {
        return UEntities.Find(x => x.QEntity.id == id);
    }

    public void SelectEntity(string id)
    {
        var entity = Entity(id);
        if (entity == null) return;
        
        var selectableObject = entity.GetComponent<SelectableObject>();
        if (selectableObject == null) return;

        selectionManager.Select(selectableObject);
    }

    public void SelectEntities(IEnumerable<Entity> entities)
    {
        var selectableObjects = entities.Select(x => x.GetComponent<SelectableObject>());
        selectionManager.SelectMany(selectableObjects);
    }

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