using System;
using System.Collections.Generic;
using BasePuzzle.FalconAnalytics.Scripts.Enum;
using PuzzleGames;
using UnityEngine;

public static class ResourceUtils
{
    public static Sprite     GetIcon(this ResourceType type)      { return ResourceManager.Instance.GetIcon(type); }
    public static AFlyObject GetFlyObject(this ResourceType type) { return ResourceManager.Instance.GetFlyObject(type); }
    public static IResource  Manager(this ResourceType type)      { return ResourceManager.Instance.GetManager(type); }
}

[DefaultExecutionOrder(-99)]
public class ResourceManager : PersistentSingleton<ResourceManager>
{
    private readonly Dictionary<ResourceType, IResource> _resourceDict = new();

    public Action<ResourceValue> OnAddResource;

    protected override void Awake()
    {
        base.Awake();

        var resources = GetComponentsInChildren<IResource>(includeInactive: true);
        foreach (var resource in resources)
        {
            RegisterResource(resource);
        }
    }

    public void RegisterResource(IResource resource) { _resourceDict[resource.Type] = resource; }

    public void Add(ResourceType type, int amount, string itemType = "", string itemId = "")
    {
        if (_resourceDict.TryGetValue(type, out var resource))
            resource.Add(amount, itemType, itemId);
    }

    public void AddFreeTime(ResourceType type, int minutes)
    {
        if (_resourceDict.TryGetValue(type, out var resource))
            resource.ActivateFreeMode(minutes);
    }

    public void AddFreeTime(ResourceValue resourceValue) { AddFreeTime(resourceValue.type, resourceValue.value); }

    public IResource GetManager(ResourceType type)
    {
        if (_resourceDict.TryGetValue(type, out var resource))
            return resource;

        Debug.LogError($"{type} Manager is not found");
        return null;
    }

    public void Add(ResourceValue resourceValue, string itemType = "", string itemId = "") { Add(resourceValue.type, resourceValue.value, itemType, itemId); }

    public void Subtract(ResourceValue resourceValue, string itemType = "", string itemId = "") { Subtract(resourceValue.type, resourceValue.value, itemType, itemId); }

    public void Add(List<ResourceValue> resources)
    {
        foreach (var r in resources)
        {
            Add(r);
        }
    }

    public void AddFreeTime(List<ResourceValue> resources)
    {
        foreach (var r in resources)
        {
            AddFreeTime(r);
        }
    }

    public void Subtract(ResourceType type, int amount, string itemType = "", string itemId = "")
    {
        if (_resourceDict.TryGetValue(type, out var resource))
            resource.Subtract(amount, itemType, itemId);
    }

    public Sprite GetIcon(ResourceType type) { return _resourceDict.TryGetValue(type, out var res) ? res.GetIcon() : null; }

    public AFlyObject GetFlyObject(ResourceType type) { return _resourceDict.TryGetValue(type, out var res) ? res.GetFlyObject() : null; }

    public int GetAmount(ResourceType type) { return _resourceDict.TryGetValue(type, out var res) ? res.GetAmount() : 0; }
    public void ReleaseUI()
    {
        foreach (var r in _resourceDict)
        {
            r.Value.ReleaseUI();
        }
    }
}