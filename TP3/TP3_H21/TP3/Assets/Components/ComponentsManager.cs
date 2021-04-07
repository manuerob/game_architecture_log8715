using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Singleton<V> where V : new()
{
    private static bool isInitiated = false;
    private static V _instance;
    public static V Instance {
        get {
            if (!isInitiated)
            {
                isInitiated = true;
                _instance = new V();
            }
            return _instance;
        }
    }
    protected Singleton() { }
}

internal class ComponentsManager : Singleton<ComponentsManager>
{
    public ComponentsManager()
    {
        ClearComponents<EntityComponent>();
        ClearComponents<ShapeComponent>();
        ClearComponents<PlayerComponent>();
        ClearComponents<SpawnInfo>();
        ClearComponents<MessagingInfo>();
        ClearComponents<ReplicationMessage>();
        ClearComponents<InputMessage>();
        ClearComponents<CollisionEventComponent>();
    }

    private Dictionary<Type, Dictionary<uint, IComponent>> _allComponents = new Dictionary<Type, Dictionary<uint, IComponent>>();
    private List<InputMessage> _inputHistory = new List<InputMessage>();
    private List<DelayMessage> _delayHistory = new List<DelayMessage>();
    private List<InputMessage> _inputQueue = new List<InputMessage>();
    private List<DelayMessage> _delayQueue = new List<DelayMessage>();

    public const int maxEntities = 2000;

    public int delayMs = 0;

    public void DebugPrint()
    {
        string toPrint = "";
        var allComponents = Instance.DebugGetAllComponents();
        foreach (var type in allComponents)
        {
            toPrint += $"{type}: \n";
            foreach (var component in type.Value)
            {
                toPrint += $"\t{component.Key}: {component.Value}\n";
            }
            toPrint += "\n";
        }
        Debug.Log(toPrint);
    }

    // CRUD
    public void SetComponent<T>(EntityComponent entityID, IComponent component) where T : IComponent
    {
        if (!_allComponents.ContainsKey(typeof(T)))
        {
            _allComponents[typeof(T)] = new Dictionary<uint, IComponent>();
        }
        _allComponents[typeof(T)][entityID] = component;
    }
    public void RemoveComponent<T>(EntityComponent entityID) where T : IComponent
    {
        _allComponents[typeof(T)].Remove(entityID);
    }
    public T GetComponent<T>(EntityComponent entityID) where T : IComponent
    {
        return (T)_allComponents[typeof(T)][entityID];
    }
    public bool TryGetComponent<T>(EntityComponent entityID, out T component) where T : IComponent
    {
        if (_allComponents.ContainsKey(typeof(T)))
        {
            if (_allComponents[typeof(T)].ContainsKey(entityID))
            {
                component = (T)_allComponents[typeof(T)][entityID];
                return true;
            }
        }
        component = default;
        return false;
    }

    public bool EntityContains<T>(EntityComponent entity) where T : IComponent
    {
        return _allComponents[typeof(T)].ContainsKey(entity);
    }

    public void ClearComponents<T>() where T : IComponent
    {
        if (!_allComponents.ContainsKey(typeof(T)))
        {
            _allComponents.Add(typeof(T), new Dictionary<uint, IComponent>());
        }
        else
        {
            _allComponents[typeof(T)].Clear();
        }
    }

    public void ForEach<T1>(Action<EntityComponent, T1> lambda) where T1 : IComponent
    {
        var allEntities = _allComponents[typeof(EntityComponent)].Values;
        foreach (EntityComponent entity in allEntities)
        {
            if (!_allComponents[typeof(T1)].ContainsKey(entity))
            {
                continue;
            }
            lambda(entity, (T1)_allComponents[typeof(T1)][entity]);
        }
    }

    public void ForEach<T1, T2>(Action<EntityComponent, T1, T2> lambda) where T1 : IComponent where T2 : IComponent
    {
        var allEntities = _allComponents[typeof(EntityComponent)].Values;
        foreach (EntityComponent entity in allEntities)
        {
            if (!_allComponents[typeof(T1)].ContainsKey(entity) ||
                !_allComponents[typeof(T2)].ContainsKey(entity)
                )
            {
                continue;
            }
            lambda(entity, (T1)_allComponents[typeof(T1)][entity], (T2)_allComponents[typeof(T2)][entity]);
        }
    }

    public void ForEach<T1, T2, T3>(Action<EntityComponent, T1, T2, T3> lambda) where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        var allEntities = _allComponents[typeof(EntityComponent)].Values;
        foreach (EntityComponent entity in allEntities)
        {
            if (!_allComponents[typeof(T1)].ContainsKey(entity) ||
                !_allComponents[typeof(T2)].ContainsKey(entity) ||
                !_allComponents[typeof(T3)].ContainsKey(entity)
                )
            {
                continue;
            }
            lambda(entity, (T1)_allComponents[typeof(T1)][entity], (T2)_allComponents[typeof(T2)][entity], (T3)_allComponents[typeof(T3)][entity]);
        }
    }

    public void ForEach<T1, T2, T3, T4>(Action<EntityComponent, T1, T2, T3, T4> lambda) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
    {
        var allEntities = _allComponents[typeof(EntityComponent)].Values;
        foreach (EntityComponent entity in allEntities)
        {
            if (!_allComponents[typeof(T1)].ContainsKey(entity) ||
                !_allComponents[typeof(T2)].ContainsKey(entity) ||
                !_allComponents[typeof(T3)].ContainsKey(entity) ||
                !_allComponents[typeof(T4)].ContainsKey(entity)
                )
            {
                continue;
            }
            lambda(entity, (T1)_allComponents[typeof(T1)][entity], (T2)_allComponents[typeof(T2)][entity], (T3)_allComponents[typeof(T3)][entity], (T4)_allComponents[typeof(T4)][entity]);
        }
    }

    public Dictionary<Type, Dictionary<uint, IComponent>> DebugGetAllComponents()
    {
        return _allComponents;
    }

    public List<InputMessage> GetInputHistory()
    {
        return _inputHistory;
    }

    public List<DelayMessage> GetDelayHistory()
    {
        return _delayHistory;
    }

    public List<InputMessage> DebugGetInputQueue()
    {
        return _inputQueue;
    }

    public void AddToInputHistory(InputMessage msg)
    {
        _inputHistory.Add(msg);
    }
    
    public void AddToDelayHistory(DelayMessage msg)
    {
        _delayHistory.Add(msg);
    }

    public InputMessage GetFirstFromInputHistory()
    {
        return _inputHistory[0];
    }

    public DelayMessage GetFirstFromDelayHistory()
    {
        return _delayHistory[0];
    }

    public InputMessage GetLastFromInputHistory()
    {
        return _inputHistory[_inputHistory.Count - 1];
    }

    public void RemoveFirstFromInputHistory()
    {
        _inputHistory.RemoveAt(0);
    }

    public void RemoveFirstFromDelayHistory()
    {
        _delayHistory.RemoveAt(0);
    }

    public void AddToInputQueue(InputMessage msg)
    {
        _inputQueue.Add(msg);
    }

    public void AddToDelayQueue(DelayMessage msg)
    {
        _delayQueue.Add(msg);
    }

    public int InputQueueCount => _inputQueue.Count;
    public int DelayQueueCount => _delayQueue.Count;
    public int InputHistoryCount => _inputHistory.Count;
    public int DelayHistoryCount => _delayHistory.Count;

    public InputMessage GetFromInputQueue()
    {
        InputMessage msg = _inputQueue[0];
        _inputQueue.RemoveAt(0);

        return msg;
    }

    public bool InputQueueContainsEntity(uint entityId)
    { 
        return _inputQueue.FindIndex(x => x.entityId == entityId) > 0;
    }

    public bool DelayQueueContainsEntity(uint entityId)
    {
        return _delayQueue.FindIndex(x => x.entityId == entityId) > 0;
    }

    public InputMessage GetFromInputQueue(uint entityId)
    {
        int index = _inputQueue.FindIndex(x => x.entityId == entityId);
        InputMessage msg = _inputQueue[index];
        _inputQueue.RemoveAt(index);

        return msg;
    }    
    
    public DelayMessage GetFromDelayQueue()
    {
        DelayMessage msg = _delayQueue[0];
        _delayQueue.RemoveAt(0);

        return msg;
    }

    public DelayMessage GetFromDelayQueue(uint entityId)
    {
        int index = _delayQueue.FindIndex(x => x.entityId == entityId);
        DelayMessage msg = _delayQueue[index];
        _delayQueue.RemoveAt(index);

        return msg;
    }

    static uint inputMessageCount = 0;
    static uint delayMessageCount = 0;
    public uint GetInputId => inputMessageCount++;
    public uint GetDelayMessageId => delayMessageCount++;
}
