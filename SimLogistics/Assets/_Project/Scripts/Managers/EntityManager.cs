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
    [SerializeField] protected SelectionManager selectionManager;
    [SerializeField] protected MapManager mapManager;
    [SerializeField] protected SimulationManager simulationManager;
    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable entityOffsetX;
    [SerializeField] private FloatVariable entityOffsetY;
    [SerializeField] private FloatVariable spawnHeight;
    [SerializeField] private FloatVariable spawnDelay;

    [SerializeField] private float lerpSpeed = .5f;
    [SerializeField] private float rotateSpeed = .3f;

    public List<Entity> UEntities { get; set; } = new List<Entity>();

    public float EntityXToWorldX(float x) => tileSize.Value * x + entityOffsetX.Value;
    public float EntityYToWorldZ(float y) => -(tileSize.Value * y + entityOffsetY.Value);


    // Interface
    public abstract IEnumerable<TQEntity> QEntities { get; }
    public abstract Entity EntityPrefab(TQEntity qEntity);

    public Entity EntityById(string id)
    {
        return UEntities.Find(x => x.QEntity.id == id);
    }

    public void SelectEntity(string id)
    {
        var entity = EntityById(id);
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
    
    public static float RotationFromDirection(Vector3 direction)
    {
        return direction.x switch
        {
            1f => 90f,
            -1f => -90f,
            _ => Math.Abs(direction.z + 1f) < float.Epsilon ? 180f : 0f
        };
    }

    protected void VisitWaypoints(QEntity qEntity, GameObject uEntity, Queue<QWaypoint> waypoints)
    {
        if (!waypoints.Any()) return;

        var waypoint = waypoints.Dequeue();
        var entityTransform = uEntity.transform;
        var position = entityTransform.position;

        // Assume we are moving to the next waypoint centroid
        var destX = waypoint.x;
        var destY = waypoint.y;

        // We may not have reached the final waypoint
        if (!waypoints.Any())
        {
            if ((Math.Abs(qEntity.x - waypoint.x) > float.Epsilon) || (Math.Abs(qEntity.y - waypoint.y) > float.Epsilon))
            {
                destX = qEntity.x;
                destY = qEntity.y;
            }
        }

        var newPosX = EntityXToWorldX(destX);
        var newPosZ = EntityYToWorldZ(destY);
        
        var newPosition = new Vector3(newPosX, position.y, newPosZ);

        var dir = (newPosition - position).normalized;
        
        // Skip if we're already there
        if (Math.Abs(dir.magnitude) < float.Epsilon)
        {
            VisitWaypoints(qEntity, uEntity, waypoints);
            return;
        }
        
        var rfd = RotationFromDirection(dir);
        var rotation = entityTransform.rotation.eulerAngles;

        Func<LTDescr> lerp = () => LeanTween.move(uEntity, newPosition, lerpSpeed).setOnComplete(() =>
        {
            VisitWaypoints(qEntity, uEntity, waypoints);
        });

        if (Math.Abs(rfd - rotation.y) > float.Epsilon)
        {
            LeanTween.rotateY(uEntity, rfd, rotateSpeed).setOnComplete(() => lerp());
        }
        else
        {
            lerp();
        }
    }
}