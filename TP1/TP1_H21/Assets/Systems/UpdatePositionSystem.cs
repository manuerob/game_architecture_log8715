using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePositionSystem : ISystem
{
    public string Name => "UpdatePositionSystem";

    public void UpdateSystem()
    {
        foreach (KeyValuePair<EntityComponent, SpeedComponent> speed in World.Instance.GetComponentsDict<SpeedComponent>())
        {
            PositionComponent position = World.Instance.GetComponent<PositionComponent>(speed.Key);

            position.Position += speed.Value.Speed * Time.deltaTime;

            ECSManager.Instance.UpdateShapePosition(speed.Key.id, position.Position);
        }
    }
}
