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
    [SerializeField] private FloatVariable tileOffsetX;
    [SerializeField] private FloatVariable tileOffsetY;
    [SerializeField] private FloatVariable spawnHeight;
    [SerializeField] private FloatVariable spawnDelay;

    [SerializeField] private GameObject sandTilePrefab;
    [SerializeField] private GameObject grassTilePrefab;
    [SerializeField] private GameObject pavementTilePrefab;
    [SerializeField] private GameObject waterTilePrefab;
    
    private QMapAndTiles QMapAndTiles { get; set; }
    private List<GameObject> _tileGameObjects = new List<GameObject>();

    public float TileSize => tileSize.Value;
    private float TileXToWorldX(float x) => tileSize.Value * x + tileOffsetX.Value;
    private float TileYToWorldZ(float y) => -(tileSize.Value * y + tileOffsetY.Value);
    
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
            connectionManager.ApiEndpoint,
            query, 
            queryName,
            qMapAndTiles =>
        {
            // print($"MapAndTiles query result: {qMapAndTiles}");

            QMapAndTiles = qMapAndTiles;
            
            onLoaded.Invoke();
        });
    }

    private IEnumerator Co_Spawn()
    {
        foreach (var qTile in QMapAndTiles.tiles)
        {
            var tilePrefab = ResolveTilePrefab(qTile.type.id);
            if (tilePrefab is null) continue;
            
            var posX = TileXToWorldX(qTile.x);
            var posZ = TileYToWorldZ(qTile.y);
            
            var tile = Instantiate(tilePrefab, new Vector3(posX, spawnHeight.Value * tileSize.Value, posZ), Quaternion.identity);
            _tileGameObjects.Add(tile);
            
            yield return new WaitForSeconds(spawnDelay.Value);
        }
        
        onSpawned.Invoke();
    }

    private GameObject ResolveTilePrefab(string id)
    {
        return id switch
        {
            "Grass" => grassTilePrefab,
            "Sand" => sandTilePrefab,
            "Pavement" => pavementTilePrefab,
            "Water" => waterTilePrefab,
            _ => null
        };
    }
}