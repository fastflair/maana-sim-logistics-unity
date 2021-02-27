using System;
using System.Collections;
using System.Collections.Generic;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class VehicleManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onVehiclesSpawned;

    [SerializeField] private MapManager mapManager;
    [SerializeField] private MapSettings mapSettings;
    [SerializeField] private StringVariable simName;
    [SerializeField] private GameObject truck;
    [SerializeField] private GameObject plane;
    [SerializeField] private GameObject ship;

    public List<QVehicle> QVehicles { get; private set; }
    public List<GameObject> VehicleGameObjects { get; private set; }

    private float _startX;
    private float _startZ;

    public void Spawn()
    {
        _startX = -(mapSettings.tileSizeX * (mapManager.QMapAndTiles.map.tilesX / 2));
        _startZ = mapSettings.tileSizeZ * (mapManager.QMapAndTiles.map.tilesY / 2);

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

    private void QueryQCallback(GraphQLResponse response)
    {
        if (response.Errors != null) throw new Exception("GraphQL errors: " + response.Errors);

        Destroy();

        VehicleGameObjects = new List<GameObject>();
        QVehicles = response.GetList<QVehicle>("mapVehicles");

        StartCoroutine(Co_Spawn());
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
            VehicleGameObjects.Add(vehicleGameObject);

            yield return new WaitForSeconds(mapSettings.spawnDelay);
        }

        onVehiclesSpawned.Invoke();
    }

    private GameObject SpawnVehicle(GameObject vehicle, float tileX, float tileY)
    {
        var posX = _startX + mapSettings.tileSizeX * tileX;
        var posZ = _startZ - mapSettings.tileSizeZ * tileY;

        // print("entity:posX: " + posX + " (" + tileX + ")");
        // print("entity:posZ: " + posZ + " (" + tileY + ")");

        return Instantiate(vehicle, new Vector3(posX, mapSettings.startY, posZ), Quaternion.identity);
    }
}