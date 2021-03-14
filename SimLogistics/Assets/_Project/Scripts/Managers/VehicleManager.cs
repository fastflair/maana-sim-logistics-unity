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

    [SerializeField] private float lerpSpeed = 3;

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

            // entity.QEntity = QEntities.First(qEntity => qEntity.id == entity.QEntity.id);
            // var entityTransform = entity.transform;
            // var newPosX = EntityXToWorldX(entity.QEntity.x);
            // var newPosZ = EntityYToWorldZ(entity.QEntity.y);
            // if (!(Math.Abs(newPosX - entityTransform.position.x) > float.Epsilon) &&
            //     !(Math.Abs(newPosZ - entityTransform.position.z) > float.Epsilon))
            // {
            //     // print($"[{name}] {SimulationManager.FormatEntityIdDisplay(entity.QEntity.id)} - no change.");
            //     continue;
            // }
            //
            // // print($"[{name}] {SimulationManager.FormatEntityIdDisplay(entity.QEntity.id)} -> ({newPosX}, {newPosZ})");
            //
            // var newPosition = new Vector3(newPosX, entityTransform.position.y, newPosZ);
            // StartCoroutine(LerpPosition(entityTransform, newPosition, lerpSpeed));
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
}