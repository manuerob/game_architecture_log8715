using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTimesToExecuteSimulationSystem : ISystem
{
    public string Name => "UpdateTimesToExecuteSimulationSystem";

    public void UpdateSystem()
    {
        foreach (KeyValuePair<EntityComponent, TimesToExecuteSimulationComponent> timesToExecute in World.Instance.GetComponentsDict<TimesToExecuteSimulationComponent>())
        {
            UpdatePerEntity(timesToExecute.Key);
        }
    }

    public void UpdatePerEntity(EntityComponent entity)
    {
        PositionComponent position = World.Instance.GetComponent<PositionComponent>(entity);
        TimesToExecuteSimulationComponent timesToExecute = World.Instance.GetComponent<TimesToExecuteSimulationComponent>(entity);

        if (position.Position.y > World.Instance.HalfScreenHeightPosition && timesToExecute.TimesToExecute > 1)
        {
            timesToExecute.TimesToExecute--;
        }
        else
        {
            timesToExecute.TimesToExecute = 0;
            World.Instance.RemoveComponent<CanUpdateSimulationComponent>(entity);
        }
    }
}
