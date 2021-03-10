using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class EditorPage<T> : MonoBehaviour
    where T : QEntity
{
    [SerializeField] protected ScenarioEditor scenarioEditor;

    [SerializeField] protected ButtonItem entityItemPrefab;
    [SerializeField] protected Transform entityItemList;

    [SerializeField] protected InputFieldItem entityId;
    [SerializeField] protected InputFieldItem entitySim;
    [SerializeField] protected InputFieldItem entityX;
    [SerializeField] protected InputFieldItem entityY;
    [SerializeField] protected InputFieldItem entitySteps;

    [SerializeField] protected ButtonItem resourceItemPrefab;
    [SerializeField] protected Transform resourceItemList;

    [SerializeField] protected InputFieldItem resourceId;
    [SerializeField] protected InputFieldItem resourceSim;
    [SerializeField] protected InputFieldItem resourceType;
    [SerializeField] protected InputFieldItem resourceCapacity;
    [SerializeField] protected InputFieldItem resourceQuantity;
    [SerializeField] protected InputFieldItem resourceBasePricePerUnit;
    [SerializeField] protected InputFieldItem resourceAdjustedPricePerUnit;
    [SerializeField] protected InputFieldItem resourceScarcitySurchargeFactor;
    [SerializeField] protected InputFieldItem resourceConsumptionRate;
    [SerializeField] protected InputFieldItem resourceReplenishRate;

    //  TODO: add notes editor
    // [SerializeField] protected NoteItem noteItemPrefab;
    // [SerializeField] protected Transform noteItemList;

    [SerializeField] protected InputDialog inputDialog;
    [SerializeField] protected ConfirmationDialog confirmationDialog;

    protected List<T> Entities;
    protected T CurrentEntity;

    // Conversion helpers
    // ------------------

    protected static string ConvertFloat(float x) => x.ToString(CultureInfo.CurrentCulture);
    protected static float ConvertFloat(string x) => float.Parse(x);
    protected static string ConvertInt(int x) => x.ToString(CultureInfo.CurrentCulture);
    protected static int ConvertInt(string x) => int.Parse(x);


    // Event handlers
    // --------------

    // --- Entities
    public void OnEntitiesLoaded(IEnumerable<T> entities)
    {
        
        ClearPage();

        Entities = entities.ToList();
        print($"OnCitiesLoaded: {Entities.Count}");

        foreach (var city in Entities)
            AddEntityItemToList(city);
    }

    public void OnAddEntity()
    {
        var dialog = scenarioEditor.uiManager.ShowDialogPrefab<InputDialog>(inputDialog);
        dialog.Title = "Add Entity";
        dialog.Label = "ID";
        dialog.onOkay.AddListener(() =>
        {
            print("onOkay");
            AddNewEntity(dialog.Value);
        });
    }

    public void OnSaveEntity()
    {
        if (CurrentEntity == null)
        {
            scenarioEditor.Status = "No entity selected to save.";
            return;
        }

        scenarioEditor.Status = $"Saving {CurrentEntity.id}...";
        SaveBaseEntity(CurrentEntity);
        SaveEntity(CurrentEntity, isSaved =>
        {
            if (isSaved)
            {
                scenarioEditor.Status = $"Saved {CurrentEntity.id}";
                CurrentEntity = null;
                scenarioEditor.LoadCities();
            }
            else
            {
                scenarioEditor.Status = $"{CurrentEntity.id} NOT saved.";
            }
        });
    }

    public void OnDeleteEntity()
    {
        if (CurrentEntity == null)
        {
            scenarioEditor.Status = "No entity selected to delete.";
            return;
        }

        var dialog = scenarioEditor.uiManager.ShowDialogPrefab<ConfirmationDialog>(confirmationDialog);
        dialog.Title = "Delete Entity";
        dialog.Text = $"Are you sure you want to delete entity: {CurrentEntity.id}?";
        dialog.onOkay.AddListener(() =>
        {
            scenarioEditor.Status = $"Deleting {CurrentEntity.id}...";
            DeleteEntity(CurrentEntity, isDeleted =>
            {
                if (isDeleted)
                {
                    scenarioEditor.Status = $"Deleted {CurrentEntity.id}";
                    CurrentEntity = null;
                    scenarioEditor.LoadCities();
                }
                else
                {
                    scenarioEditor.Status = $"{CurrentEntity.id} NOT deleted.";
                }
            });
        });
    }

    // --- Resources

    public void OnAddResource()
    {
        print("OnAddResource");
    }

    public void OnSaveResource()
    {
        print("OnSaveResource");
        var resource = new QResource
        {
            id = resourceId.Value,
            sim = resourceSim.Value,
            type = new QResourceTypeEnum {id = resourceType.Value},
            capacity = ConvertFloat(resourceCapacity.Value),
            quantity = ConvertFloat(resourceQuantity.Value),
            basePricePerUnit = ConvertFloat(resourceBasePricePerUnit.Value),
            adjustedPricePerUnit = ConvertFloat(resourceAdjustedPricePerUnit.Value),
            scarcitySurchargeFactor = ConvertFloat(resourceScarcitySurchargeFactor.Value),
            consumptionRate = ConvertFloat(resourceConsumptionRate.Value),
            replenishRate = ConvertFloat(resourceReplenishRate.Value),
        };
        print($"Saving resource: {resource}");
    }

    public void OnDeleteResource()
    {
        print("OnDeleteResource");
    }

    // Overrides
    // ---------

    // Create a new specialized entity (e.g., city)
    protected abstract void AddNewEntity(string id);

    // Populate the form with a specialized entity (e.g., city)
    protected abstract void PopulateEntityInputFields(T entity);

    // Save a specialized entity (e.g., city)
    protected abstract void SaveEntity(T entity, Action<bool> callback);

    // Delete a specialized entity (e.g., city
    protected abstract void DeleteEntity(T entity, Action<bool> callback);

    // Entity management
    // -----------------

    protected ButtonItem AddEntityItemToList(T entity)
    {
        Debug.Assert(entityItemPrefab != null);

        var item = Instantiate(entityItemPrefab, entityItemList, false);
        item.Label = entity.id;
        item.onClick.AddListener(() => PopulateAllEntityInputFields(entity));
        return item;
    }

    protected void PopulateAllEntityInputFields(T entity)
    {
        CurrentEntity = entity;
        PopulateBaseEntityInputFields(entity);
        PopulateEntityInputFields(entity);
    }

    private void PopulateBaseEntityInputFields(T entity)
    {
        entityId.Value = entity?.id;
        entitySim.Value = entity?.sim;
        entityX.Value = ConvertFloat(entity?.x  ?? 0f);
        entityY.Value = ConvertFloat(entity?.y  ?? 0f);
        entitySteps.Value = ConvertInt(entity?.steps ?? 0);
    }

    private void SaveBaseEntity(T entity)
    {
        entity.id = entityId.Value;
        entity.sim = entitySim.Value;
        entity.x = ConvertFloat(entityX.Value);
        entity.y = ConvertFloat(entityY.Value);
        entity.steps = ConvertInt(entitySteps.Value);
    }

    // Resource management
    // -------------------
    protected IEnumerable<ButtonItem> AddResourceItems(IEnumerable<QResource> resources)
    {
        ClearList(resourceItemList);

        if (resources == null) yield break;
        foreach (var resource in resources)
            yield return AddResourceItemToList(resource);
    }

    private ButtonItem AddResourceItemToList(QResource resource)
    {
        var resourceItem = Instantiate(resourceItemPrefab, resourceItemList, false);
        resourceItem.Label = resource.id;
        resourceItem.onClick.AddListener(() => PopulateResourceInputFields(resource));
        return resourceItem;
    }

    private void PopulateResourceInputFields(QResource resource)
    {
        resourceId.Value = resource?.id;
        resourceSim.Value = resource?.sim;
        resourceType.Value = resource?.type.id;
        resourceCapacity.Value =
            ConvertFloat(resource?.capacity ?? 0f);
        resourceQuantity.Value =
            ConvertFloat(resource?.quantity ?? 0f);
        resourceBasePricePerUnit.Value =
            ConvertFloat(resource?.basePricePerUnit ?? 0f);
        resourceAdjustedPricePerUnit.Value =
            ConvertFloat(resource?.adjustedPricePerUnit ?? 0f);
        resourceScarcitySurchargeFactor.Value =
            ConvertFloat(resource?.scarcitySurchargeFactor ?? 0f);
        resourceConsumptionRate.Value =
            ConvertFloat(resource?.consumptionRate ?? 0f);
        resourceReplenishRate.Value = ConvertFloat(resource?.replenishRate ?? 0f);
    }

    // Cleanup
    // -------

    protected void ClearPage()
    {
        ClearList(entityItemList);
        ClearList(resourceItemList);
        // TODO
        // ClearList(noteItemList);
        PopulateAllEntityInputFields(null);
        PopulateResourceInputFields(null);
    }

    private static void ClearList(IEnumerable list)
    {
        foreach (Transform child in list)
            Destroy(child.gameObject);
    }

}