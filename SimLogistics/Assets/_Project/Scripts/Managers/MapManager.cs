using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private GameObject sandTile00Prefab;

    [SerializeField] private GameObject grassTile00Prefab;

    [SerializeField] private GameObject pavementTile00Prefab;
    [SerializeField] private GameObject pavementTile01Prefab;

    [SerializeField] private GameObject waterTile00Prefab;
    [SerializeField] private GameObject waterTile01Prefab;

    private QMapAndTiles QMapAndTiles { get; set; }
    private List<GameObject> _tileGameObjects = new List<GameObject>();

    public float TileYRot(float x, float y)
    {
        return (
            from tile in QMapAndTiles.tiles
            where
                Math.Abs(tile.x - x) < Single.Epsilon
                && Math.Abs(tile.y - y) < Single.Epsilon
            select tile.yRot
        ).FirstOrDefault();
    }

    public Quaternion TileRotation(float x, float y)
    {
        var yRot = TileYRot(x, y);
        var quaternion = Quaternion.identity;
        if (yRot != 0f)
        {
            quaternion.eulerAngles = new Vector3(0f, yRot, 0f);
        }

        return quaternion;
    }

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
        foreach (var tile in from qTile in QMapAndTiles.tiles
            let tilePrefab = ResolveTilePrefab(qTile.type.id)
            where tilePrefab is { }
            let posX = TileXToWorldX(qTile.x)
            let posZ = TileYToWorldZ(qTile.y)
            let quaternion = TileRotation(qTile.x, qTile.y)
            let position = new Vector3(posX, spawnHeight.Value * tileSize.Value, posZ)
            select Instantiate(tilePrefab, position , quaternion))
        {
            _tileGameObjects.Add(tile);

            yield return new WaitForSeconds(spawnDelay.Value);
        }

        onSpawned.Invoke();
    }

    private GameObject ResolveTilePrefab(string id)
    {
        return id switch
        {
            "Grass" => grassTile00Prefab,
            "Sand" => sandTile00Prefab,
            "Pavement" => pavementTile00Prefab,
            "Pavement01" => pavementTile01Prefab,
            "Water" => waterTile00Prefab,
            "Water01" => waterTile01Prefab,
            _ => null
        };
    }
}