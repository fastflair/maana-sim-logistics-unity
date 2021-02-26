using System;
using System.Collections;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;

public class TileMapSpawner : MonoBehaviour
{ 
    [SerializeField] private UnityEvent<QMapAndTiles, float, float> onTileMapSpawned;
    
    [SerializeField] private string mapName = "Default";
    [SerializeField] private float tileSizeX;
    [SerializeField] private float tileSizeZ;
    [SerializeField] private float startY;
    [SerializeField] private float spawnDelay;
    [SerializeField] private GameObject landTile;
    [SerializeField] private GameObject waterTile;

    private QMapAndTiles _qMapAndTiles;
    
    private void Start()
    {
        QueryQ();
    }

    private void QueryQ()
    {
        var query = @$"
          query {{
            mapAndTiles(map: ""{mapName}"") {{
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

        GraphQLManager.Instance.Query(query, callback: QueryQCallback);
    }

    private void QueryQCallback(GraphQLResponse response)
    {
        if (response.Errors != null) throw new Exception("GraphQL errors: " + response.Errors);
        
        _qMapAndTiles = response.GetValue<QMapAndTiles>("mapAndTiles");

        StartCoroutine(Co_Spawn());
    }

    private IEnumerator Co_Spawn()
    {
        var startX = -(tileSizeX * (_qMapAndTiles.map.tilesX / 2));
        var startZ = tileSizeZ * (_qMapAndTiles.map.tilesY / 2);

        foreach (var qTile in _qMapAndTiles.tiles)
        {
            var tile = qTile.type.id == "Land" ? landTile : waterTile;
            if (tile is null) continue;
            
            var posX = startX + tileSizeX * qTile.x;
            var posZ = startZ - tileSizeZ * qTile.y;
        
            // print("posX: " + posX + " (" + qTile.x + ")");
            // print("posZ: " + posZ + " (" + qTile.y + ")");
        
            Instantiate(tile, new Vector3(posX, startY, posZ), Quaternion.identity);

            yield return new WaitForSeconds(spawnDelay);
        }

        // yield return new WaitForSeconds(.5f);
        
        onTileMapSpawned.Invoke(_qMapAndTiles, tileSizeX, tileSizeZ);
    }
}