using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveStateSystem : ISystem
{
    public string Name => "SaveStateSystem";

    public void UpdateSystem()
    {
        World.Instance.backUpStates.Enqueue(new KeyValuePair<DateTime, Dictionary<Type, Dictionary<EntityComponent, ICopiableComponent>>>(DateTime.Now, World.Instance.DeepCopyComponent()));

        for(int i = 0, length =  World.Instance.backUpStates.Where(x => (DateTime.Now - x.Key).TotalSeconds >= World.Instance.coolDownInitialValue).Count(); i < length;  i++)
        {
            World.Instance.backUpStates.Dequeue();
        }
    }
}