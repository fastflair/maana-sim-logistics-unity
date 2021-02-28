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
    
    [SerializeField] private GraphQLManager apiService;
    [SerializeField] private StringVariable mapName;
    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable tileDropHeight;
    [SerializeField] private FloatVariable spawnDelay;
    [SerializeField] private GameObject landTile;
    [SerializeField] private GameObject waterTile;

    private bool TitlesComplete { get; set; }
    
    private QMapAndTiles QMapAndTiles { get; set; }
    public List<GameObject> TileGameObjects { get; private set; }
    
    [UsedImplicitly]
    public void Spawn()
    {
        QueryQ();
    }

    public void OnTitlesComplete()
    {
        TitlesComplete = true;

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
        var query = @$"
          query {{
            mapAndTiles(map: ""{mapName.Value}"") {{
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

        apiService.Query(query, callback: QueryQCallback);
    }

    private void QueryQCallback(GraphQLResponse response)
    {
        if (response.Errors != null) throw new Exception("GraphQL errors: " + response.Errors);
        
        Destroy();

        QMapAndTiles = response.GetValue<QMapAndTiles>("mapAndTiles");

        TrySpawn();
    }

    private void TrySpawn()
    {
        if (TitlesComplete && QMapAndTiles != null)
        {
            StartCoroutine(Co_Spawn());
        }
    }
    
    private IEnumerator Co_Spawn()
    {
        foreach (var qTile in QMapAndTiles.tiles)
        {
            var tile = qTile.type.id == "Land" ? landTile : waterTile;
            if (tile is null) continue;
            
            var posX = tileSize.Value * qTile.x;
            var posZ = -tileSize.Value * qTile.y;
            
            Instantiate(tile, new Vector3(posX, tileDropHeight.Value * tileSize.Value, posZ), Quaternion.identity);

            yield return new WaitForSeconds(spawnDelay.Value);
        }
        
        onMapTilesSpawned.Invoke();
    }
}