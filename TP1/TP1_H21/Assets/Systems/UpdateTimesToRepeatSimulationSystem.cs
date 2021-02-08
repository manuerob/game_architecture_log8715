using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTimesToRepeatSimulationSystem : ISystemUpdatablePerEntity
{
    public string Name => "UpdateTimesToRepeatSimulationSystem";

    public void UpdateSystem()
    {
        foreach (KeyValuePair<EntityComponent, TimesToRepeatSimulationComponent> timesToExecute in World.Instance.GetComponentsDict<TimesToRepeatSimulationComponent>())
        {
            UpdatePerEntity(timesToExecute.Key);

            timesToExecute.Value.TimesToRepeat = World.Instance.timescale;
        }
    }

    public void UpdatePerEntity(EntityComponent entity)
    {
        PositionComponent position = World.Instance.GetComponent<PositionComponent>(entity);
        TimesToRepeatSimulationComponent timesToExecute = World.Instance.GetComponent<TimesToRepeatSimulationComponent>(entity);

        if (position.Position.y > World.Instance.HalfScreenHeightPosition)
        {
            timesToExecute.TimesToRepeat--;
        }
        else
        {
            timesToExecute.TimesToRepeat = 0;
        }
    }
}
