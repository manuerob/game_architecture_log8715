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

            foreach (KeyValuePair<EntityComponent, SizeComponent> size2 in World.Instance.GetComponentsDict<SizeComponent>())
            {
                if(speed.Key.id != size2.Key.id)
                {
                    PositionComponent position2 = World.Instance.GetComponent<PositionComponent>(size2.Key);

                    if ((position2.Position - position.Position).magnitude < (size.Size + size2.Value.Size)/2)
                    {
                        size.Size /= 2;
                        speed.Value.Speed *= -1;
                        ECSManager.Instance.UpdateShapeSize(speed.Key.id, size.Size);

                        SpeedComponent speed2 = World.Instance.GetComponent<SpeedComponent>(size2.Key);

                        if (speed2 != null)
                        {
                            size2.Value.Size /= 2;
                            speed2.Speed *= -1;
                            ECSManager.Instance.UpdateShapeSize(size2.Key.id, size2.Value.Size);
                        }
                    }
                }
            }

        }
    }
}