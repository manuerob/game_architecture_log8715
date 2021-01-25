using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class World
{
    public bool isStarting = true;

    private Dictionary<Type, Dictionary<EntityComponent, IComponent>> components = new Dictionary<Type, Dictionary<EntityComponent, IComponent>>();

    public void RegisterComponentsDict<ComponentType>(Dictionary<EntityComponent, ComponentType> newComponentDict) where ComponentType : IComponent
    {
        components.Add(typeof(ComponentType), newComponentDict.ToDictionary(entry => entry.Key,
                                                                            entry => (IComponent)entry.Value));
    }

    public Dictionary<EntityComponent, ComponentType> GetComponentsDict<ComponentType>() where ComponentType : IComponent
    {
        return components[typeof(ComponentType)].ToDictionary(entry => entry.Key,
                                                              entry => (ComponentType)entry.Value);
    }

    public void RemoveComponent<ComponentType>(EntityComponent entity) where ComponentType : IComponent
    {
        if (components.ContainsKey(typeof(ComponentType)))
            components[typeof(ComponentType)].Remove(entity);
    }

    public void AddComponent<ComponentType>(EntityComponent entity, ComponentType newComponent) where ComponentType : IComponent
    {
        if (!components.ContainsKey(typeof(ComponentType)))
            RegisterComponentsDict(new Dictionary<EntityComponent, ComponentType>());
        components[typeof(ComponentType)].Add(entity, newComponent);
    }
    public ComponentType GetComponent<ComponentType>(EntityComponent index) where ComponentType : IComponent
    {
        return GetComponentsDict<ComponentType>()[index];
    }

    #region Singleton
    private static World _instance;
    public static World Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new World();
            }
            return _instance;
        }
    }
    #endregion
}

