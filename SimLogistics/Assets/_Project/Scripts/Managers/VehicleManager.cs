using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleManager : EntityManager<QVehicle>
{
    [SerializeField] private Entity truckPrefab;
    [SerializeField] private Entity planePrefab;
    [SerializeField] private Entity shipPrefab;

    [SerializeField] private float lerpSpeed = .5f;
    [SerializeField] private float rotateSpeed = .3f;

    public override IEnumerable<QVehicle> QEntities => simulationManager.CurrentState.vehicles;

    public override Entity EntityPrefab(QVehicle qVehicle)
    {
        return qVehicle.type.id switch
        {
            "Plane" => planePrefab,
            "Ship" => shipPrefab,
            "Truck" => truckPrefab,
            _ => null
        };
    }

    private static bool IsMoving(string status)
    {
        switch (status)
        {
            case "InTransit":
            case "Arrived":
            case "Repair":
                return true;
            default:
                return false;
        }
    }

    public void OnSimulationUpdate()
    {
        foreach (var vehicle in simulationManager.CurrentState.vehicles)
        {
            var transitOrder = vehicle.transitOrder;
            var status = transitOrder.status.id;
            if (!IsMoving(status)) continue;
            
            var vehicleEntity = EntityById(vehicle.id);
            var remaining = new Queue<QWaypoint>(transitOrder.visited);
            MoveThroughWaypoints(vehicle, vehicleEntity.gameObject, remaining);
        }
    }

    public static float RotationFromDirection(Vector3 direction)
    {
        return direction.x switch
        {
            1f => 90f,
            -1f => -90f,
            _ => Math.Abs(direction.z + 1f) < Single.Epsilon ? 180f : 0f
        };
    }

    private void MoveThroughWaypoints(QVehicle vehicle, GameObject go, Queue<QWaypoint> waypoints)
    {
        var vehicleTransform = go.transform;
        var position = vehicleTransform.position;
        
        if (!waypoints.Any())
        {
            var endPosition = new Vector3(
                EntityXToWorldX(vehicle.x),
                position.y,
                EntityYToWorldZ(vehicle.y));
            print($"No waypoints: ${position} {endPosition}");
            LeanTween.move(go, endPosition, lerpSpeed);
            return;
        }

        var waypoint = waypoints.Dequeue();

        var newPosX = EntityXToWorldX(waypoint.x);
        var newPosZ = EntityYToWorldZ(waypoint.y);
        
        var newPosition = new Vector3(newPosX, position.y, newPosZ);

        var dir = (newPosition - position).normalized;
        var rfd = RotationFromDirection(dir);
        var rotation = vehicleTransform.rotation.eulerAngles;

        Func<LTDescr> lerp = () => LeanTween.move(go, newPosition, lerpSpeed).setOnComplete(() =>
        {
            MoveThroughWaypoints(vehicle, go, waypoints);
        });

        if (Math.Abs(rfd - rotation.y) > float.Epsilon)
        {
            LeanTween.rotateY(go, rfd, rotateSpeed).setOnComplete(() => lerp());
        }
        else
        {
            lerp();
        }
    }
}