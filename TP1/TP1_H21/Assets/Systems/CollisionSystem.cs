using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem : ISystem
{
    public string Name => "CollisionSystem";

    public void UpdateSystem()
    {
        foreach (KeyValuePair<EntityComponent, SpeedComponent> speed in World.Instance.GetComponentsDict<SpeedComponent>())
        {
            SizeComponent size = World.Instance.GetComponent<SizeComponent>(speed.Key);
            PositionComponent position = World.Instance.GetComponent<PositionComponent>(speed.Key);

            if (World.Instance.GetComponentsDict<CanCollideComponent>().ContainsKey(speed.Key))
            {
                foreach (KeyValuePair<EntityComponent, SizeComponent> size2 in World.Instance.GetComponentsDict<SizeComponent>())
                {
                    if (speed.Key.id != size2.Key.id && World.Instance.GetComponentsDict<CanCollideComponent>().ContainsKey(size2.Key))
                    {
                        PositionComponent position2 = World.Instance.GetComponent<PositionComponent>(size2.Key);

                        if ((position2.Position - position.Position).magnitude < (size.Size + size2.Value.Size) / 2)
                        {
                            size.Size /= 2;
                            speed.Value.Speed *= -1;
                            ECSManager.Instance.UpdateShapeSize(speed.Key.id, size.Size);

                            TurnIntoGhost(size.Size, speed.Key);

                            if (World.Instance.GetComponentsDict<SpeedComponent>().ContainsKey(size2.Key))
                            {
                                size2.Value.Size /= 2;
                                SpeedComponent speed2 = World.Instance.GetComponent<SpeedComponent>(size2.Key);
                                speed2.Speed *= -1;
                                ECSManager.Instance.UpdateShapeSize(size2.Key.id, size2.Value.Size);

                                TurnIntoGhost(size2.Value.Size, size2.Key);
                            }
                        }
                    }
                }
            }
        }
    }

    private void TurnIntoGhost(float size, EntityComponent entity)
    {
        if (size < ECSManager.Instance.Config.minSize)
        {
            World.Instance.RemoveComponent<CanCollideComponent>(entity);
            World.Instance.GetComponent<ColorComponent>(entity).Color = Color.green;
            ECSManager.Instance.UpdateShapeColor(entity.id, Color.green);
        }
    }
}