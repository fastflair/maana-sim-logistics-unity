using System;
using System.Collections;
using System.Collections.Generic;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class VehicleManager : MonoBehaviour, ISelectionHandler
{
    [SerializeField] private UnityEvent onVehiclesSpawned;

    [SerializeField] private StringVariable simName;
    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable tileDropHeight;
    [SerializeField] private FloatVariable spawnDelay;
    [SerializeField] private GameObject truck;
    [SerializeField] private GameObject plane;
    [SerializeField] private GameObject ship;

    public List<QVehicle> QVehicles { get; private set; }
    public List<GameObject> VehicleGameObjects { get; private set; }

    public void Spawn()
    {
        QueryQ();
    }

    private void Destroy()
    {
        if (VehicleGameObjects is null) return;
        
        foreach (var entity in VehicleGameObjects)
        {
            Object.Destroy(entity);
        }    
    }

    private void QueryQ()
    {
        var query = @$"
          query {{
            mapVehicles(sim: ""{simName.Value}"") {{
              id
              kind
              x
              y
              status
            }}
          }}
        ";

        GraphQLManager.Instance.Query(query, callback: QueryQCallback);
    }
    
    private void QueryQCallback(GraphQLResponse response)
    {
        if (response.Errors != null) throw new Exception("GraphQL errors: " + response.Errors);

        Destroy();

        VehicleGameObjects = new List<GameObject>();
        QVehicles = response.GetList<QVehicle>("mapVehicles");

        StartCoroutine(Co_Spawn());
    }

    private GameObject VehicleGameObject(string kind)
    {
        return kind switch
        {
            "Plane" => plane,
            "Ship" => ship,
            "Truck" => truck,
            _ => null
        };
    }

    private IEnumerator Co_Spawn()
    {
        foreach (var qVehicle in QVehicles)
        {
            var vehiclePrototype = VehicleGameObject(qVehicle.kind);
            if (vehiclePrototype is null)
            {
                print("Unsupported vehicle kind: " + qVehicle.kind);
                continue;
            }

            var vehicleGameObject = SpawnVehicle(vehiclePrototype, qVehicle.x, qVehicle.y);
            var selectableObject = vehicleGameObject.AddComponent<SelectableObject>();
            selectableObject.SelectionHandler = this;
            selectableObject.id = qVehicle.id;
            VehicleGameObjects.Add(vehicleGameObject);

            yield return new WaitForSeconds(spawnDelay.Value);
        }

        onVehiclesSpawned.Invoke();
    }

    private GameObject SpawnVehicle(GameObject vehicle, float tileX, float tileY)
    {
        var posX = tileSize.Value * tileX;
        var posZ = -tileSize.Value * tileY;

        return Instantiate(vehicle, new Vector3(posX, tileDropHeight.Value * tileSize.Value, posZ), Quaternion.identity);
    }

    public void OnHoverEnter(GameObject selectedObject)
    {
        throw new NotImplementedException();
    }

    public void OnHoverExit(GameObject selectedObject)
    {
        throw new NotImplementedException();
    }

    public void OnSelect(GameObject selectedObject)
    {
        throw new NotImplementedException();
    }

    public void OnDeselect(GameObject selectedObject)
    {
        throw new NotImplementedException();
    }
}