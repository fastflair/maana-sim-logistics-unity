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

    private bool IsOkToSpawn { get; set; }
    
    private QMapAndTiles QMapAndTiles { get; set; }
    public List<GameObject> TileGameObjects { get; private set; }
    
    public void Load()
    {
        QueryQ();
    }

    public void Spawn()
    {
        IsOkToSpawn = true;

        TrySpawn();
    }
    
    private void Destroy()
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
          query {{
            {queryName}(map: ""{mapName.Value}"") {{
              map {{
                id
                tilesX
                tilesY
              }}
              tiles {{
                id
                type {{ id }}
                x
                y
              }}
            }}
          }}
        ";

        connectionManager.apiEndpoint.Query(query, callback: response =>
        {
            if (response.Errors != null)
            {
                connectionManager.RaiseQueryError(@$"[{name}] failed to query {queryName}:{Environment.NewLine}{response.Errors}");
                return;
            }

            Destroy();

            QMapAndTiles = response.GetValue<QMapAndTiles>(queryName);

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
            var tile = qTile.type.id == "Land" ? landTilePrefab : waterTilePrefab;
            if (tile is null) continue;
            
            var posX = tileSize.Value * qTile.x;
            var posZ = -tileSize.Value * qTile.y;
            
            Instantiate(tile, new Vector3(posX, tileDropHeight.Value * tileSize.Value, posZ), Quaternion.identity);

            yield return new WaitForSeconds(spawnDelay.Value);
        }
        
        onMapTilesSpawned.Invoke();
    }
}