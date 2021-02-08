using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class World
{
    public readonly float cooldownInitialValue = 2.0f;
    public readonly int timescale = 4;
    public readonly Vector2Int[] WallNormals = new Vector2Int[4]
    {
        new Vector2Int( 0,  1), // Bottom wall
        new Vector2Int( 0, -1), // Top wall
        new Vector2Int(-1,  0), // Right wall
        new Vector2Int( 1,  0), // Left wall
    };

    public Vector2[] WallCenters = new Vector2[4]
    {
        new Vector2(),
        new Vector2(),
        new Vector2(),
        new Vector2(),
    };
    public uint idCount = 0;
    public float cooldownValue = 2.0f;
    public float HalfScreenHeightPosition;
    public bool isStarting = true;

    public Queue<KeyValuePair<DateTime, Dictionary<Type, Dictionary<EntityComponent, ICopiableComponent>>>> backUpStates = new Queue<KeyValuePair<DateTime, Dictionary<Type, Dictionary<EntityComponent, ICopiableComponent>>>>();
    public List<ISystemUpdatablePerEntity> simulationSystems = new List<ISystemUpdatablePerEntity>();

    private Dictionary<Type, Dictionary<EntityComponent, ICopiableComponent>> components = new Dictionary<Type, Dictionary<EntityComponent, ICopiableComponent>>();

    public void RegisterComponentsDict<ComponentType>(Dictionary<EntityComponent, ComponentType> newComponentDict) where ComponentType : ICopiableComponent
    {
        components.Add(typeof(ComponentType), newComponentDict.ToDictionary(entry => entry.Key,
                                                                            entry => (ICopiableComponent)entry.Value));
    }

    public Dictionary<EntityComponent, ComponentType> GetComponentsDict<ComponentType>() where ComponentType : ICopiableComponent
    {
        return components[typeof(ComponentType)].ToDictionary(entry => entry.Key,
                                                              entry => (ComponentType)entry.Value);
    }

    public void RemoveComponent<ComponentType>(EntityComponent entity) where ComponentType : ICopiableComponent
    {
        if (components.ContainsKey(typeof(ComponentType)))
            components[typeof(ComponentType)].Remove(entity);
    }

    public void AddComponent<ComponentType>(EntityComponent entity, ComponentType newComponent) where ComponentType : ICopiableComponent
    {
        if (!components.ContainsKey(typeof(ComponentType)))
            RegisterComponentsDict(new Dictionary<EntityComponent, ComponentType>());
        components[typeof(ComponentType)].Add(entity, newComponent);
    }

    public ComponentType GetComponent<ComponentType>(EntityComponent index) where ComponentType : ICopiableComponent
    {
        return GetComponentsDict<ComponentType>()[index];
    }

    public Dictionary<Type, Dictionary<EntityComponent, ICopiableComponent>> DeepCopyComponent()
    {
        Dictionary<Type, Dictionary<EntityComponent, ICopiableComponent>> newDict = new Dictionary<Type, Dictionary<EntityComponent, ICopiableComponent>>();

        foreach(var componentDictionnary in components)
        {
            Dictionary<EntityComponent, ICopiableComponent> newComponentDictionnary = new Dictionary<EntityComponent, ICopiableComponent>();
            newDict.Add(componentDictionnary.Key, newComponentDictionnary);

            foreach(var component in componentDictionnary.Value)
            {
                newComponentDictionnary.Add(component.Key, (ICopiableComponent)component.Value.Clone());
            }
        }

        return newDict;
    }

    public void RestoreState()
    {
        components = backUpStates.Dequeue().Value;

        backUpStates.Clear();

        cooldownValue = cooldownInitialValue;
    }

    public uint GetNextId()
    {
        return idCount++;
    }

    public void RegisterSimulationSystems()
    {
        simulationSystems.Add(new UpdateTimesToRepeatSimulationSystem());
        simulationSystems.Add(new UpdatePositionSystem());
        simulationSystems.Add(new CollisionSystem());
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

                _instance.RegisterSimulationSystems();
            }
            return _instance;
        }
    }
    #endregion
}

