using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling<TComponent, MatchType> : MonoBehaviour where TComponent : Component
{
    [Header("[ Scriptable Object ]"), Space(6)]
    [SerializeField] protected List<TComponent> componentPrefabs;
    [SerializeField] protected List<TComponent> componentPool;
    [SerializeField] protected Transform holder;

    public virtual TComponent Spawn(MatchType matchType)
    {
        TComponent component = GetComponentByMatchType(matchType);
        if (component == null) return null;

        TComponent componentSpawn = GetPoolComponent(component, matchType);
        componentSpawn.transform.SetParent(holder);
        componentSpawn.gameObject.SetActive(true);
        return componentSpawn;
    }

    protected virtual TComponent GetComponentByMatchType(MatchType matchType)
    {
        foreach (TComponent component in componentPrefabs)
            if (CheckMatchValue(matchType, component)) return component;
        return null;
    }

    protected virtual bool CheckMatchValue(MatchType matchType, TComponent component)
    {
        //TODO: This method should have a matching criteria. Currently always returns true.
        return true;
    }

    public virtual void Despawn(TComponent component)
    {
        component.gameObject.SetActive(false);
        componentPool.Add(component);
    }

    protected virtual TComponent GetPoolComponent(TComponent componentPrefab, MatchType matchType)
    {
        foreach (TComponent component in componentPool)
            if (CheckMatchValue(matchType, component) && !component.gameObject.activeSelf)
            {
                componentPool.Remove(component);
                return component;
            }

        TComponent newComponent = Instantiate(componentPrefab);
        newComponent.name = componentPrefab.name;
        return newComponent;
    }

    public virtual TComponent GetRandomPrefab()
    {
        int keyObject = UnityEngine.Random.Range(0, componentPrefabs.Count);
        return componentPrefabs[keyObject];
    }

    public virtual MatchType GetRandomEnumValue()
    {
        Array values = Enum.GetValues(typeof(MatchType));
        System.Random random = new System.Random();
        return (MatchType)values.GetValue(random.Next(values.Length));
    }
}
