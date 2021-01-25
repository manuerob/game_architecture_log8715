using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem : ISystem
{
    public string Name => "CollisionSystem";

    public void UpdateSystem()
    {
        foreach (KeyValuePair<EntityComponent, ColorComponent> color in World.Instance.GetComponentsDict<ColorComponent>())
        {

            //ECSManager.Instance.UpdateShapePosition(position.Key.id, position.Value.Position);
        }
    }
}