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
        return UEntities.Find(x => x.entityId == id);
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
            entity.entityId = qEntity.id;
            UEntities.Add(entity);

            yield return new WaitForSeconds(spawnDelay.Value);
        }

        onSpawned.Invoke();
    }

    private static bool FloatEq(float val, float target) => Math.Abs(val - target) < 0.1f;

    private static float RotationFromDirection(Vector3 direction)
    {
        if (FloatEq(direction.x, 1f)) return 90f;
        if (FloatEq(direction.x, -1f)) return -90f;
        if (FloatEq(direction.z, -1f)) return 180f;
        if (FloatEq(direction.x, .7f) && FloatEq(direction.z, .7f)) return 45f;
        if (FloatEq(direction.x, -.7f) && FloatEq(direction.z, .7f)) return 135f;
        if (FloatEq(direction.x, -.7f) && FloatEq(direction.z, -.7f)) return 225f;
        if (FloatEq(direction.x, .7f) && FloatEq(direction.z, -.7f)) return 315f;
        return 0f;
    }

    protected void VisitWaypoints(QVehicle qVehicle, GameObject uEntity, Queue<QWaypoint> waypoints)
    {
        if (!waypoints.Any()) return;

        var waypoint = waypoints.Dequeue();
        // print($"Waypoint: {waypoint}");
        var entityTransform = uEntity.transform;
        var position = entityTransform.position;
        // print($"Position: {position}");
        var nextWaypoint = waypoints.Any() ? waypoints.Peek() : null;
        if (nextWaypoint != null)
        {
            var cwpPos = new Vector3(EntityXToWorldX(waypoint.x), EntityYToWorldZ(waypoint.y));
            var nwpPos = new Vector3(EntityXToWorldX(nextWaypoint.x), EntityYToWorldZ(nextWaypoint.y));
            var distanceToCurrent = (position - cwpPos).magnitude;
            var distanceToNext = (position - nwpPos).magnitude;
            var isNextCloser = distanceToNext <= distanceToCurrent;
            // print($"dtoc: {distanceToCurrent}, dton: {distanceToNext}, isNextCloser: {isNextCloser}");
            if (isNextCloser)
            {
                VisitWaypoints(qVehicle, uEntity, waypoints);
                return;
            }
        }

        // Assume we are moving to the next waypoint centroid
        var destX = waypoint.x;
        var destY = waypoint.y;

        QDock dock = null;

        // Special processing for final waypoint
        if (!waypoints.Any() && !qVehicle.transitOrder.waypoints.Any())
        {
            // print($"Final waypoint");

            var tile = mapManager.FindTile(waypoint.x, waypoint.y);
            if (tile != null)
            {
                dock = tile.docks.Find(x => x.vehicleType == qVehicle.type.id);
                // print($"Dock: {dock}");
            }

            if (dock == null)
            {
                // print($"no dock");

                // We may not have reached the final waypoint
                if ((Math.Abs(qVehicle.x - waypoint.x) > float.Epsilon) ||
                    (Math.Abs(qVehicle.y - waypoint.y) > float.Epsilon))
                {
                    // print("forcing move to entity position");
                    destX = qVehicle.x;
                    destY = qVehicle.y;
                }
            }
        }

        var newPosX = EntityXToWorldX(destX);
        var newPosZ = EntityYToWorldZ(destY);

        if (dock != null)
        {
            newPosX += dock.xOffset;
            newPosZ -= dock.yOffset;
        }

        var newPosition = new Vector3(newPosX, position.y, newPosZ);
        var dir = (newPosition - position).normalized;
        // print($"New position: {newPosition}, dir: {dir}");

        // Skip if we're already there
        if (Math.Abs(dir.magnitude) < float.Epsilon)
        {
            // print($"Already there");
            VisitWaypoints(qVehicle, uEntity, waypoints);
            return;
        }

        var yRot = dock?.yRot ?? RotationFromDirection(dir);

        var rotation = entityTransform.rotation.eulerAngles;

        LTDescr Lerp() => LeanTween.move(uEntity, newPosition, lerpSpeed)
            .setOnComplete(() => { VisitWaypoints(qVehicle, uEntity, waypoints); });

        if (Math.Abs(yRot - rotation.y) > float.Epsilon)
        {
            // print($"Rotating: {yRot}");
            LeanTween.rotateY(uEntity, yRot, rotateSpeed).setOnComplete(() => Lerp());
        }
        else
        {
            // print("Not rotating");
            Lerp();
        }
    }
}