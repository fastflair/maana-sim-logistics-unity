using System;
using System.Collections;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;

public class MapManager : MonoBehaviour
{ 
    [SerializeField] private UnityEvent onMapTilesSpawned;
    
    [SerializeField] private MapSettings mapSettings;
    [SerializeField] private GameObject landTile;
    [SerializeField] private GameObject waterTile;

    public QMapAndTiles QMapAndTiles { get; private set; }

    private void Start()
    {
        QueryQ();
    }

    private void QueryQ()
    {
        var query = @$"
          query {{
            mapAndTiles(map: ""{mapSettings.mapName}"") {{
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
        
        QMapAndTiles = response.GetValue<QMapAndTiles>("mapAndTiles");

        StartCoroutine(Co_Spawn());
    }

    private IEnumerator Co_Spawn()
    {
        var startX = -(mapSettings.tileSizeX * (QMapAndTiles.map.tilesX / 2));
        var startZ = mapSettings.tileSizeZ * (QMapAndTiles.map.tilesY / 2);

        foreach (var qTile in QMapAndTiles.tiles)
        {
            var tile = qTile.type.id == "Land" ? landTile : waterTile;
            if (tile is null) continue;
            
            var posX = startX + mapSettings.tileSizeX * qTile.x;
            var posZ = startZ - mapSettings.tileSizeZ * qTile.y;
        
            // print("posX: " + posX + " (" + qTile.x + ")");
            // print("posZ: " + posZ + " (" + qTile.y + ")");
        
            Instantiate(tile, new Vector3(posX, mapSettings.startY, posZ), Quaternion.identity);

            yield return new WaitForSeconds(mapSettings.spawnDelay);
        }
        
        onMapTilesSpawned.Invoke();
    }
}