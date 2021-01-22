using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePositionSystem : ISystem
{
    public string Name => "UpdatePositionSystem";

    public void UpdateSystem()
    {
        foreach (KeyValuePair<EntityComponent, PositionComponent> position in World.Instance.GetComponentsDict<PositionComponent>())
        {
            position.Value.Position += World.Instance.GetComponent<SpeedComponent>(position.Key).Speed * Time.deltaTime;

            ECSManager.Instance.UpdateShapePosition(position.Key.id, position.Value.Position);
        }
    }
}
