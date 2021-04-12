using System.Collections.Generic;
using UnityEngine;

public class BounceBackSystem : IExtrapolatableSystem
{
    public string Name
    {
        get
        {
            return GetType().Name;
        }
    }

    public void ExtrapolateEntities()
    {
        ComponentsManager.Instance.ForEach<CollisionEventComponent, ShapeComponent, ReplicationMessage>((entity, collisionEventComponent, shapeComponent, replicationMessage) =>
        {
            Vector2 speed = shapeComponent.speed;

            shapeComponent.speed = -shapeComponent.speed;
            ComponentsManager.Instance.SetComponent<ShapeComponent>(entity, shapeComponent);
        });
    }

    public void UpdateSystem()
    {
        ComponentsManager.Instance.ForEach<CollisionEventComponent, ShapeComponent>((entity, collisionEventComponent, shapeComponent) =>
        {
            Vector2 speed = shapeComponent.speed;

            shapeComponent.speed = -shapeComponent.speed;
            ComponentsManager.Instance.SetComponent<ShapeComponent>(entity, shapeComponent);
        });
    }
}