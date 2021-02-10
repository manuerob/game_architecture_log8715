using System.Collections.Generic;
using UnityEngine;

public class UpdatePositionSystem : ISystem
{
    public string Name => "UpdatePositionSystem";

    public void UpdateSystem()
    {
        foreach (KeyValuePair<EntityComponent, SpeedComponent> speed in World.Instance.GetComponentsDict<SpeedComponent>())
        {
            if(World.Instance.HasComponent<CanUpdateSimulationComponent>(speed.Key))
                UpdatePerEntity(speed.Key);
        }
    }
    public void UpdatePerEntity(EntityComponent entity)
    {
        PositionComponent position = World.Instance.GetComponent<PositionComponent>(entity);
        SpeedComponent speed = World.Instance.GetComponent<SpeedComponent>(entity);

        position.Position += speed.Speed * Time.deltaTime;
    }
}
