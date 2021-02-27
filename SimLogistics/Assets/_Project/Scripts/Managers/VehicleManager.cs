using System;
using System.Collections;
using System.Collections.Generic;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;

public class VehicleManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onVehiclesSpawned;

    [SerializeField] private string simName;
    [SerializeField] private float startY;
    [SerializeField] private float spawnDelay;
    [SerializeField] private GameObject truck;
    [SerializeField] private GameObject plane;
    [SerializeField] private GameObject ship;

    private QMapAndTiles _qMapAndTiles;
    private float _startX;
    private float _startZ;
    private float _tileSizeX;
    private float _tileSizeZ;
    private List<QVehicle> _qVehicles;

    public void Spawn(QMapAndTiles qMap, float tileSizeX, float tileSizeZ)
    {
        _qMapAndTiles = qMap;
        _tileSizeX = tileSizeX;
        _tileSizeZ = tileSizeZ;
        _startX = -(_tileSizeX * (_qMapAndTiles.map.tilesX / 2));
        _startZ = _tileSizeZ * (_qMapAndTiles.map.tilesY / 2);

        QueryQ();
    }

    private void QueryQ()
    {
        var query = @$"
          query {{
            mapVehicles(sim: ""{simName}"") {{
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

        _qVehicles = response.GetList<QVehicle>("mapVehicles");

        StartCoroutine(Co_Spawn());
    }

    private IEnumerator Co_Spawn()
    {
        foreach (var qVehicle in _qVehicles)
        {
            var vehicle = VehicleGameObject(qVehicle.kind);
            if (vehicle is null)
            {
                print("Unsupported vehicle kind: " + qVehicle.kind);
                continue;
            }

            SpawnVehicle(vehicle, qVehicle.x, qVehicle.y);

            yield return new WaitForSeconds(spawnDelay);
        }

        onVehiclesSpawned.Invoke();
    }

    private void SpawnVehicle(GameObject vehicle, float tileX, float tileY)
    {
        var posX = _startX + _tileSizeX * tileX;
        var posZ = _startZ - _tileSizeZ * tileY;

        // print("entity:posX: " + posX + " (" + tileX + ")");
        // print("entity:posZ: " + posZ + " (" + tileY + ")");

        Instantiate(vehicle, new Vector3(posX, startY, posZ), Quaternion.identity);
    }
}