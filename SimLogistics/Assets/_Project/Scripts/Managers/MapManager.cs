using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class MapManager : MonoBehaviour
{ 
    [SerializeField] private UnityEvent onMapTilesSpawned;
    
    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private StringVariable mapName;
    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable tileDropHeight;
    [SerializeField] private FloatVariable spawnDelay;
    [SerializeField] private GameObject landTilePrefab;
    [SerializeField] private GameObject waterTilePrefab;

    public bool IsOkToSpawn { get; set; }
    
    private QMapAndTiles QMapAndTiles { get; set; }
    private List<GameObject> TileGameObjects { get; set; }

    public void OkToSpawn()
    {
        if (IsOkToSpawn) return;
        IsOkToSpawn = true;
        TrySpawn();
    }
    
    public void Spawn()
    {
        QueryQ();
    }
    
    public void Destroy()
    {
        if (TileGameObjects is null) return;
        
        foreach (var tile in TileGameObjects)
        {
            Object.Destroy(tile);
        }    
    }

    private void QueryQ()
    {
        const string queryName = "mapAndTiles";
        var query = @$"
          {QMapFragment.data}
          {QTileFragment.data}
          {QMapAndTilesFragment.data}
          query {{
            {queryName}(map: ""{mapName.Value}"") {{
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
            Destroy();
            
            TileGameObjects = new List<GameObject>();

            QMapAndTiles = qMapAndTiles;

            TrySpawn();
        });
    }

    private void TrySpawn()
    {
        if (IsOkToSpawn && QMapAndTiles != null)
        {
            StartCoroutine(Co_Spawn());
        }
    }
    
    private IEnumerator Co_Spawn()
    {
        foreach (var qTile in QMapAndTiles.tiles)
        {
            var tilePrefab = qTile.type.id == "Land" ? landTilePrefab : waterTilePrefab;
            if (tilePrefab is null) continue;
            
            var posX = tileSize.Value * qTile.x;
            var posZ = -tileSize.Value * qTile.y;
            
            var tile = Instantiate(tilePrefab, new Vector3(posX, tileDropHeight.Value * tileSize.Value, posZ), Quaternion.identity);
            TileGameObjects.Add(tile);
            
            yield return new WaitForSeconds(spawnDelay.Value);
        }
        
        onMapTilesSpawned.Invoke();
    }
}