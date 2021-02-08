using System;
using System.Collections.Generic;
using System.Linq;

public class SaveStateSystem : ISystem
{
    public string Name => "SaveStateSystem";

    public void UpdateSystem()
    {
        World.Instance.backUpStates.Enqueue(new KeyValuePair<DateTime, Dictionary<Type, Dictionary<EntityComponent, ICopiableComponent>>>(DateTime.Now, World.Instance.DeepCopyComponent()));

        for (int i = 0, length = World.Instance.backUpStates.Where(x => (DateTime.Now - x.Key).TotalSeconds >= World.Instance.cooldownInitialValue).Count(); i < length; i++)
        {
            World.Instance.backUpStates.Dequeue();
        }
    }
}