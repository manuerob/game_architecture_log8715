//#define BAD_PERF // TODO CHANGEZ MOI. Mettre en commentaire pour utiliser votre propre structure

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

#if BAD_PERF
using InnerType = System.Collections.Generic.Dictionary<uint, IComponent>;
using AllComponents = System.Collections.Generic.Dictionary<uint, System.Collections.Generic.Dictionary<uint, IComponent>>;
#else
using InnerType = SeqPool<IComponent>; // TODO CHANGEZ MOI, UTILISEZ VOTRE PROPRE TYPE ICI
using AllComponents = SeqPoolManager;//System.Collections.Generic.Dictionary<uint, SeqPool<IComponent>>; // TODO CHANGEZ MOI, UTILISEZ VOTRE PROPRE TYPE ICI
#endif

// Appeler GetHashCode sur un Type est couteux. Cette classe sert a precalculer le hashcode
public static class TypeRegistry<T> where T : IComponent
{
    public static uint typeID = (uint)Mathf.Abs(default(T).GetRandomNumber()) % ComponentsManager.maxEntities;
}

public class Singleton<V> where V : new()
{
    private static bool isInitiated = false;
    private static V _instance;
    public static V Instance
    {
        get
        {
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
    private AllComponents _allComponents = new AllComponents();

    public const int maxEntities = 2000;

    public void DebugPrint()
    {
        string toPrint = "";
        var allComponents = Instance.DebugGetAllComponents();
#if BAD_PERF
        foreach (var type in allComponents)
        {
            toPrint += $"{type}: \n";
            foreach (var component in type.Value)
            {
                toPrint += $"\t{component.Key}: {component.Value}\n";
            }
#else
        foreach (var type in allComponents.seqPools)
        {
            for (int i = 0, length = type.AllocatedCount; i < length; i++)
            {
                if (type.ContainsKey(i))
                {
                    toPrint += $"\t{i}: {type.GetValue(i)}\n";
                }
            }
#endif
            toPrint += "\n";
        }
        Debug.Log(toPrint);
    }

    // CRUD
    public void SetComponent<T>(EntityComponent entityID, IComponent component) where T : IComponent
    {
        if (!_allComponents.ContainsKey(TypeRegistry<T>.typeID))
        {
            //_allComponents[TypeRegistry<T>.typeID] = new Dictionary<uint, IComponent>();
            //_allComponents[TypeRegistry<T>.typeID] = new InnerType();
            _allComponents.Add(TypeRegistry<T>.typeID, new SeqPool<T>());
        }
#if BAD_PERF
        _allComponents[TypeRegistry<T>.typeID][entityID] = component;
#else
        ((InnerType)_allComponents[TypeRegistry<T>.typeID])[entityID] = component;
#endif
    }
    public void RemoveComponent<T>(EntityComponent entityID) where T : IComponent
    {
        _allComponents[TypeRegistry<T>.typeID].Remove(entityID);
    }
    public T GetComponent<T>(EntityComponent entityID) where T : IComponent
    {
#if BAD_PERF
        return (T)_allComponents[TypeRegistry<T>.typeID][entityID];
#else
        return (T)((InnerType)_allComponents[TypeRegistry<T>.typeID])[entityID];
#endif
    }
    public bool TryGetComponent<T>(EntityComponent entityID, out T component) where T : IComponent
    {
        if (_allComponents.ContainsKey(TypeRegistry<T>.typeID))
        {
            if (_allComponents[TypeRegistry<T>.typeID].ContainsKey(entityID))
            {
#if BAD_PERF
                component = (T)_allComponents[TypeRegistry<T>.typeID][entityID];
#else
                component = (T)((InnerType)_allComponents[TypeRegistry<T>.typeID])[entityID];
#endif
                return true;
            }
        }
        component = default;
        return false;
    }

    public bool EntityContains<T>(EntityComponent entity) where T : IComponent
    {
        return _allComponents[TypeRegistry<T>.typeID].ContainsKey(entity);
    }

    public void ClearComponents<T>() where T : IComponent
    {
        if (!_allComponents.ContainsKey(TypeRegistry<T>.typeID))
        {
            _allComponents.Add(TypeRegistry<T>.typeID, new SeqPool<T>());
        }
        else
        {
           _allComponents[TypeRegistry<T>.typeID].Clear();
        }
    }

    public void ForEach<T1>(Action<EntityComponent, T1> lambda) where T1 : IComponent
    {
#if BAD_PERF
        var allEntities = _allComponents[TypeRegistry<EntityComponent>.typeID].Values;
        foreach (EntityComponent entity in allEntities)
        {
            if (!_allComponents[TypeRegistry<T1>.typeID].ContainsKey(entity))
            {
                continue;
            }
            lambda(entity, (T1)_allComponents[TypeRegistry<T1>.typeID][entity]);
        }
#else
        SeqPool<EntityComponent> entities = (SeqPool<EntityComponent>)_allComponents[TypeRegistry<EntityComponent>.typeID];
        for (int i = 0, length = entities.Count; i < length; i++)
        {
            EntityComponent entity = entities[i];
            var T1SeqPool = _allComponents[TypeRegistry<T1>.typeID];

            if (T1SeqPool.ContainsKey(entity))
            {
                lambda(entity, ((SeqPool<T1>)T1SeqPool)[entity]);
            }
        }
#endif
    }

    public void ForEach<T1, T2>(Action<EntityComponent, T1, T2> lambda) where T1 : IComponent where T2 : IComponent
    {
#if BAD_PERF
        var allEntities = _allComponents[TypeRegistry<EntityComponent>.typeID].Values;
        foreach(EntityComponent entity in allEntities)
        {
            if (!_allComponents[TypeRegistry<T1>.typeID].ContainsKey(entity) ||
                !_allComponents[TypeRegistry<T2>.typeID].ContainsKey(entity)
                )
            {
                continue;
            }
            lambda(entity, (T1)_allComponents[TypeRegistry<T1>.typeID][entity], (T2)_allComponents[TypeRegistry<T2>.typeID][entity]);
        }
#else
        SeqPool<EntityComponent> entities = (SeqPool<EntityComponent>)_allComponents[TypeRegistry<EntityComponent>.typeID];
        for (int i = 0, length = entities.Count; i < length; i++)
        {
            EntityComponent entity = (EntityComponent)entities[i];
            var T1SeqPool = _allComponents[TypeRegistry<T1>.typeID];
            var T2SeqPool = _allComponents[TypeRegistry<T2>.typeID];

            if (T1SeqPool.ContainsKey(entity)
                && T2SeqPool.ContainsKey(entity))
            {
                lambda(entity, ((SeqPool<T1>)T1SeqPool)[entity], ((SeqPool<T2>)T2SeqPool)[entity]);
            }
        }
#endif
    }

    public void ForEach<T1, T2, T3>(Action<EntityComponent, T1, T2, T3> lambda) where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
#if BAD_PERF
        var allEntities = _allComponents[TypeRegistry<EntityComponent>.typeID].Values;
        foreach (EntityComponent entity in allEntities)
        {
            if (!_allComponents[TypeRegistry<T1>.typeID].ContainsKey(entity) ||
                !_allComponents[TypeRegistry<T2>.typeID].ContainsKey(entity) ||
                !_allComponents[TypeRegistry<T3>.typeID].ContainsKey(entity)
                )
            {
                continue;
            }
            lambda(entity, (T1)_allComponents[TypeRegistry<T1>.typeID][entity], (T2)_allComponents[TypeRegistry<T2>.typeID][entity], (T3)_allComponents[TypeRegistry<T3>.typeID][entity]);
        }
#else
        SeqPool<EntityComponent> entities = (SeqPool<EntityComponent>)_allComponents[TypeRegistry<EntityComponent>.typeID];
        for (int i = 0, length = entities.Count; i < length; i++)
        {
            EntityComponent entity = (EntityComponent)entities[i];
            var T1SeqPool = _allComponents[TypeRegistry<T1>.typeID];
            var T2SeqPool = _allComponents[TypeRegistry<T2>.typeID];
            var T3SeqPool = _allComponents[TypeRegistry<T3>.typeID];

            if (T1SeqPool.ContainsKey(entity)
                && T2SeqPool.ContainsKey(entity)
                && T3SeqPool.ContainsKey(entity))
            {
                lambda(entity, ((SeqPool<T1>)T1SeqPool)[entity], ((SeqPool<T2>)T2SeqPool)[entity], ((SeqPool<T3>)T3SeqPool)[entity]);
            }
        }
#endif
    }

    public void ForEach<T1, T2, T3, T4>(Action<EntityComponent, T1, T2, T3, T4> lambda) where T1 : IComponent where T2 : IComponent where T3 : IComponent where T4 : IComponent
    {
#if BAD_PERF
        var allEntities = _allComponents[TypeRegistry<EntityComponent>.typeID].Values;
        foreach (EntityComponent entity in allEntities)
        {
            if (!_allComponents[TypeRegistry<T1>.typeID].ContainsKey(entity) ||
                !_allComponents[TypeRegistry<T2>.typeID].ContainsKey(entity) ||
                !_allComponents[TypeRegistry<T3>.typeID].ContainsKey(entity) ||
                !_allComponents[TypeRegistry<T4>.typeID].ContainsKey(entity)
                )
            {
                continue;
            }
            lambda(entity, (T1)_allComponents[TypeRegistry<T1>.typeID][entity], (T2)_allComponents[TypeRegistry<T2>.typeID][entity], (T3)_allComponents[TypeRegistry<T3>.typeID][entity], (T4)_allComponents[TypeRegistry<T4>.typeID][entity]);
        }
#else
        SeqPool<EntityComponent> entities = (SeqPool<EntityComponent>)_allComponents[TypeRegistry<EntityComponent>.typeID];
        for (int i = 0, length = entities.Count; i < length; i++)
        {
            EntityComponent entity = (EntityComponent)entities[i];
            var T1SeqPool = _allComponents[TypeRegistry<T1>.typeID];
            var T2SeqPool = _allComponents[TypeRegistry<T2>.typeID];
            var T3SeqPool = _allComponents[TypeRegistry<T3>.typeID];
            var T4SeqPool = _allComponents[TypeRegistry<T4>.typeID];

            if (T1SeqPool.ContainsKey(entity)
                && T2SeqPool.ContainsKey(entity)
                && T3SeqPool.ContainsKey(entity)
                && T4SeqPool.ContainsKey(entity))
            {
                lambda(entity, ((SeqPool<T1>)T1SeqPool)[entity], ((SeqPool<T2>)T2SeqPool)[entity], ((SeqPool<T3>)T3SeqPool)[entity], ((SeqPool<T4>)T4SeqPool)[entity]);
            }
        }
#endif
    }

    public AllComponents DebugGetAllComponents()
    {
        return _allComponents;
    }
}

public class SeqPoolManager
{
    public List<ISeqPool> seqPools;

    public SeqPoolManager()
    {
        seqPools = new List<ISeqPool>();
    }

    public ISeqPool this[uint key] => seqPools.First(x => x.TypeId() == key);

    internal void Add<T>(uint typeID, SeqPool<T> seqPool) where T : IComponent
    {
        seqPools.Add(seqPool);
    }

    internal bool ContainsKey(uint typeID)
    {
        foreach (ISeqPool seqPool in seqPools)
        {
            if (seqPool.TypeId() == typeID)
                return true;
        }

        return false;
    }
}

public interface ISeqPool
{
    int AllocatedCount { get; }

    void Clear();
    bool ContainsKey(EntityComponent entity);
    bool ContainsKey(int i);
    IComponent GetValue(int i);
    void Remove(EntityComponent entityID);
    uint TypeId();
    
}

// 1. Remplacer le InnerType par le seqpool (implémenter le seqpool, remplacer avec le define badperfs)
// 2. Tester pour voir si on réduit le load de 40%
// 3. Remplacer le AllComponents par quelque chose d'autre que le dict de <uint, pool>
public class SeqPool<T> : ISeqPool where T:IComponent
{
    private T[] _arrayItems;
    private int[] _indirectionTable;
    private int _count;
    private uint _typeId;

    public bool ContainsKey(EntityComponent key) => _indirectionTable[key.id] != -1;
    public bool ContainsKey(int key) => _indirectionTable[key] != -1;
    public int Count => _count;
    public int AllocatedCount => _indirectionTable.Length;
    public uint TypeId() => _typeId;

    public T this[EntityComponent key]
    {
        get => GetValue(key);
        set => SetValue(key, value);
    }

    public T this[int key]
    {
        get => (T)GetValue(key);
    }

    public SeqPool(int initialAllocationSize = 500)
    {
        _arrayItems = new T[initialAllocationSize];
        _indirectionTable = Enumerable.Repeat(-1, initialAllocationSize).ToArray();

        _count = 0;
        _typeId = TypeRegistry<T>.typeID;
    }

    private void SetValue(EntityComponent key, T value)
    {
        if (ContainsKey(key))
        {
            _arrayItems[_indirectionTable[key]] = value;
        }
        else
        {
            AddValue(key, value);
        }
    }

    private void AddValue(EntityComponent key, T value)
    {
        int poolAllocationSize = _indirectionTable.Length;
        if (key.id > poolAllocationSize)
        {
            int[] newIndirections = Enumerable.Repeat(-1, poolAllocationSize * 2).ToArray();
            T[] newValuesArray = new T[poolAllocationSize * 2];

            for (int i = 0; i < poolAllocationSize; i++)
            {
                newValuesArray[i] = _arrayItems[i];
                newIndirections[i] = _indirectionTable[i];
            }
            _arrayItems = newValuesArray;
            _indirectionTable = newIndirections;
        }

        _indirectionTable[key.id] = _count;
        _arrayItems[_count] = value;

        _count++;
    }

    public T GetValue(EntityComponent key)
    {
        return _arrayItems[_indirectionTable[key.id]];
    }

    public IComponent GetValue(int key)
    {
        return _arrayItems[_indirectionTable[key]];
    }

    public void Clear()
    {
        for (int i = 0, length = _indirectionTable.Length; i < length; i++)
        {
            _indirectionTable[i] = -1;
        }
        _count = 0;
    }

    public void Remove(EntityComponent key)
    {
        if (key.id > _indirectionTable.Length
            || !ContainsKey(key))
        {
            return;
        }

        int positionToReplace = _indirectionTable[key.id];

        _arrayItems[positionToReplace] = _arrayItems[_count - 1];
        _count--;
        _indirectionTable[key.id] = -1;

        _indirectionTable[Array.IndexOf(_indirectionTable, _count)] = positionToReplace;
    }
}
