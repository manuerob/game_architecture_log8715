using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Config;

public class InitializationSystem : ISystem
{
    public string Name => "InitializationSystem";

    public void StartSystem()
    {
        for (int i = 0, length = ECSManager.Instance.Config.allShapesToSpawn.Count; i < length; i++)
        {
            // Create entities and components
            World.Instance.AddComponent(new CircleComponent((uint)i, ECSManager.Instance.Config.allShapesToSpawn[i]));
            World.Instance.AddComponent(new PositionComponent((uint)i, ECSManager.Instance.Config.allShapesToSpawn[i].initialPos));
            World.Instance.AddComponent(new SpeedComponent((uint)i, ECSManager.Instance.Config.allShapesToSpawn[i].initialSpeed));
        }

        foreach (CircleComponent circle in World.Instance.GetComponentsList<CircleComponent>())
        {
            // Instantiate Circles
            ECSManager.Instance.CreateShape(circle.id, circle.ShapeConfig);
        }
    }

    public void UpdateSystem()
    {
        if (World.Instance.isStarting) // mettre le bool dans le singleton
        {
            // initialiser toutes les composantes
            World.Instance.isStarting = false;

            StartSystem();
        }
    }
}
