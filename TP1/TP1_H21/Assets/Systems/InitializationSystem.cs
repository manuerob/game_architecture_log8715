using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Config;

/// <summary>
/// Mettre le 1/4 static avec la couleur
/// </summary>
public class InitializationSystem : ISystem
{
    public string Name => "InitializationSystem";

    public void StartSystem()
    {
        for (int i = 0, length = ECSManager.Instance.Config.allShapesToSpawn.Count; i < length; i++)
        {
            // Create entities and components
            EntityComponent entity = new EntityComponent{ id = (uint)i };
            CircleComponent circleComponent = new CircleComponent{ shapeConfig = ECSManager.Instance.Config.allShapesToSpawn[i] };
            PositionComponent positionComponent = new PositionComponent{ Position = ECSManager.Instance.Config.allShapesToSpawn[i].initialPos };
            SpeedComponent speedComponent = new SpeedComponent{ Speed = ECSManager.Instance.Config.allShapesToSpawn[i].initialSpeed };

            World.Instance.AddComponent(entity, circleComponent);
            World.Instance.AddComponent(entity, positionComponent);
            World.Instance.AddComponent(entity, speedComponent);
        }

        foreach (KeyValuePair<EntityComponent, CircleComponent> circle in World.Instance.GetComponentsDict<CircleComponent>())
        {
            // Instantiate Circles
            ECSManager.Instance.CreateShape(circle.Key.id, circle.Value.shapeConfig);
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
