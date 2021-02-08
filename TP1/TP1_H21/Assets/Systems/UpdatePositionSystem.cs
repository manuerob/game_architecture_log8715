using System.Collections.Generic;
using UnityEngine;

public class UpdatePositionSystem : ISystemUpdatablePerEntity
{
    public string Name => "UpdatePositionSystem";

    public void UpdateSystem()
    {
        foreach (KeyValuePair<EntityComponent, SpeedComponent> speed in World.Instance.GetComponentsDict<SpeedComponent>())
        {
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
