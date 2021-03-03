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

    [SerializeField] private ConnectionManager connectionManager;
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
        Destroy();
        QueryQ();
    }

    public void Destroy()
    {
        if (VehicleGameObjects is null) return;
        
        foreach (var entity in VehicleGameObjects)
        {
            Object.Destroy(entity);
        }    
    }

    private void QueryQ()
    {
        const string queryName = "mapVehicles";
        var query = @$"
          query {{
            {queryName}(sim: ""{simName.Value}"") {{
              id
              kind
              x
              y
              status
            }}
          }}
        ";

        connectionManager.QueryRaiseOnError<List<QVehicle>>(
            connectionManager.apiEndpoint,
            query,
            queryName,
            vehicles =>
            {
                Destroy();

                VehicleGameObjects = new List<GameObject>();

                QVehicles = vehicles;

                StartCoroutine(Co_Spawn());
            });
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

    public void OnHoverEnter(GameObject obj)
    {
        print($"OnHoverEnter: {obj.name}");
    }

    public void OnHoverExit(GameObject obj)
    {
        print($"OnHoverExit: {obj.name}");
    }

    public void OnSelect(GameObject obj)
    {
        print($"OnSelect: {obj.name}");
    }
}