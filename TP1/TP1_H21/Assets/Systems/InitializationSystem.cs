using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Config;

public class InitializationSystem : ISystem
{
    public string Name => "InitializationSystem";

    // petit hack temporaire, remplacer par singleton
    bool isStarting = true;

    public void StartSystem()
    {
        for (int i = 0, length = ECSManager.Instance.Config.allShapesToSpawn.Count; i < length; i++)
        {
            // Create entities and components
            EntityManager.Instance.AddCircleComponent(new CircleComponent((uint)i, ECSManager.Instance.Config.allShapesToSpawn[i]));
        }

        foreach (CircleComponent circle in EntityManager.Instance.CircleComponents)
        {
            // Instantiate Circles
            ECSManager.Instance.CreateShape(circle.id, circle.ShapeConfig);
        }
    }

    public void UpdateSystem()
    {
        if (isStarting)// mettre le bool dans le singleton
        {

            // initialiser toutes les composantes
            isStarting = false;

            StartSystem();
        }
    }
}
