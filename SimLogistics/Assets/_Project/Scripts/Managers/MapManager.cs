using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class MapManager : MonoBehaviour
{ 
    [SerializeField] private UnityEvent onLoaded;
    [SerializeField] private UnityEvent onSpawned;
    
    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private SimulationManager simulationManager;
    
    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable spawnHeight;
    [SerializeField] private FloatVariable spawnDelay;
    [SerializeField] private GameObject landTilePrefab;
    [SerializeField] private GameObject waterTilePrefab;
    
    private QMapAndTiles QMapAndTiles { get; set; }
    private List<GameObject> _tileGameObjects = new List<GameObject>();

    public void Load()
    {
        QueryQ();
    }
    
    public void Spawn()
    {
        Destroy();

        StartCoroutine(Co_Spawn());
    }
    
    public void Destroy()
    {
        if (_tileGameObjects is null) return;
        
        foreach (var tile in _tileGameObjects)
        {
            Object.Destroy(tile);
        }
        
        _tileGameObjects = new List<GameObject>();
    }

    public void OnUpdate()
    {
    }
    
    private void QueryQ()
    {
        const string queryName = "mapAndTiles";
        var query = @$"
          {QMapFragment.data}
          {QTileFragment.data}
          {QMapAndTilesFragment.data}
          query {{
            {queryName}(map: ""{SimulationManager.MapName}"") {{
              ...mapAndTilesData
            }}
          }}
        ";

        connectionManager.QueryRaiseOnError<QMapAndTiles>(
            connectionManager.apiEndpoint,
            query, 
            queryName,
            qMapAndTiles =>
        {
            QMapAndTiles = qMapAndTiles;
            
            onLoaded.Invoke();
        });
    }

    private IEnumerator Co_Spawn()
    {
        foreach (var qTile in QMapAndTiles.tiles)
        {
            var tilePrefab = qTile.type.id == "Land" ? landTilePrefab : waterTilePrefab;
            if (tilePrefab is null) continue;
            
            var posX = tileSize.Value * qTile.x;
            var posZ = -tileSize.Value * qTile.y;
            
            var tile = Instantiate(tilePrefab, new Vector3(posX, spawnHeight.Value * tileSize.Value, posZ), Quaternion.identity);
            _tileGameObjects.Add(tile);
            
            yield return new WaitForSeconds(spawnDelay.Value);
        }
        
        onSpawned.Invoke();
    }
}