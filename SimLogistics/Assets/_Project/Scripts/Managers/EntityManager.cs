using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Maana.GraphQL;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class EntityManager : MonoBehaviour, ISelectionHandler
{
    [SerializeField] private UnityEvent onEntitiesSpawned;

    [SerializeField] private StringVariable simName;
    [SerializeField] private FloatVariable tileSize;
    [SerializeField] private FloatVariable tileDropHeight;
    [SerializeField] private FloatVariable spawnDelay;
    [SerializeField] private GameObject city;
    [SerializeField] private GameObject airport;
    [SerializeField] private GameObject port;
    [SerializeField] private GameObject truckDepot;
    [SerializeField] private GameObject factory;
    [SerializeField] private GameObject warehouse;
    [SerializeField] private GameObject farm;
    [SerializeField] private GameObject forest;
    [SerializeField] private GameObject lumberMill;
    [SerializeField] private GameObject coalMine;
    [SerializeField] private GameObject powerPlant;
    [SerializeField] private GameObject cotton;
    [SerializeField] private GameObject textileMill;
    [SerializeField] private GameObject oilWell;
    [SerializeField] private GameObject oilRefinery;

    public List<QMapEntity> QMapEntities { get; private set; }
    public List<GameObject> EntityGameObjects { get; private set; }

    [UsedImplicitly]
    public void Spawn()
    {
        QueryQ();
    }

    private void Destroy()
    {
        if (EntityGameObjects is null) return;
        
        foreach (var entity in EntityGameObjects)
        {
            Object.Destroy(entity);
        }    
    }
    
    private void QueryQ()
    {
        var query = @$"
          query {{
            mapEntities(sim: ""{simName.Value}"") {{
              id
              type
              kind
              x
              y
            }}
          }}
        ";

        GraphQLManager.Instance.Query(query, callback: QueryQCallback);
    }
    
    private void QueryQCallback(GraphQLResponse response)
    {
        if (response.Errors != null) throw new Exception("GraphQL errors: " + response.Errors);

        Destroy();
        
        EntityGameObjects = new List<GameObject>();
        QMapEntities = response.GetList<QMapEntity>("mapEntities");

        StartCoroutine(Co_Spawn());
    }

    private GameObject EntityGameObject(string kind)
    {
        return kind switch
        {
            "City" => city,
            "Airport" => airport,
            "Port" => port,
            "TruckDepot" => truckDepot,
            "Factory" => factory,
            "Warehouse" => warehouse,
            "Farm" => farm,
            "Forest" => forest,
            "LumberMill" => lumberMill,
            "CoalMine" => coalMine,
            "PowerPlant" => powerPlant,
            "Cotton" => cotton,
            "TextileMill" => textileMill,
            "OilWell" => oilWell,
            "OilRefinery" => oilRefinery,
            _ => null
        };
    }

    private IEnumerator Co_Spawn()
    {
        foreach (var qMapEntity in QMapEntities)
        {
            var entityPrototype = EntityGameObject(qMapEntity.kind);
            if (entityPrototype is null)
            {
                print("Unsupported entity kind: " + qMapEntity.kind);
                continue;
            }

            var entityGameObject = SpawnEntity(entityPrototype, qMapEntity.x, qMapEntity.y);
            var selectableObject = entityGameObject.AddComponent<SelectableObject>();
            selectableObject.SelectionHandler = this;
            selectableObject.id = qMapEntity.id;
            EntityGameObjects.Add(entityGameObject);
            
            yield return new WaitForSeconds(spawnDelay.Value);
        }

        onEntitiesSpawned.Invoke();
    }

    private GameObject SpawnEntity(GameObject entity, float tileX, float tileY)
    {
        var posX = tileSize.Value * tileX;
        var posZ = -tileSize.Value * tileY;

        return Instantiate(entity, new Vector3(posX, tileDropHeight.Value * tileSize.Value, posZ), Quaternion.identity);
    }

    public void OnHoverEnter(GameObject selectedObject)
    {
        throw new NotImplementedException();
    }

    public void OnHoverExit(GameObject selectedObject)
    {
        throw new NotImplementedException();
    }

    public void OnSelect(GameObject selectedObject)
    {
        throw new NotImplementedException();
    }

    public void OnDeselect(GameObject selectedObject)
    {
        throw new NotImplementedException();
    }
}
