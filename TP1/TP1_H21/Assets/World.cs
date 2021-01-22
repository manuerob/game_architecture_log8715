using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// À rendre plus meilleur!
/// 
/// (Polymorphisme pour les listes? Listes de listes?)
/// 
/// 
/// Dict<Type(:ICompoennt), Dict<(id),Icomponent>>
/// 
/// GetList<ICompoient> -> Dict<(id),Icomponent>>
/// </summary>
public class World
{
    public bool isStarting = true;

    private Dictionary<Type, List<IComponent>> components = new Dictionary<Type, List<IComponent>>();

    public void RegisterComponentsDict<ComponentType>(List<ComponentType> newComponentDict) where ComponentType : IComponent
    {
        components.Add(typeof(ComponentType), newComponentDict.Cast<IComponent>().ToList());
    }

    public List<IComponent> GetComponentsList<ComponentType>() where ComponentType : IComponent
    {  
        return components[typeof(ComponentType)];
    }

    public void AddComponent<ComponentType>(ComponentType newComponent) where ComponentType : IComponent
    {
        if (!components.ContainsKey(typeof(ComponentType)))
            RegisterComponentsDict(new List<ComponentType>());
        GetComponentsList<ComponentType>().Add(newComponent);
    }
    public ComponentType GetComponent<ComponentType>(int index) where ComponentType : IComponent
    { 
        return (ComponentType)GetComponentsList<ComponentType>()[index];
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

