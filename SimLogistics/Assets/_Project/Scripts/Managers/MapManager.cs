using System;
using System.Collections;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;

public class MapManager : MonoBehaviour
{ 
    [SerializeField] private UnityEvent onMapTilesSpawned;
    
    [SerializeField] private StringVariable mapName;
    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable tileDropHeight;
    [SerializeField] private FloatVariable spawnDelay;
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