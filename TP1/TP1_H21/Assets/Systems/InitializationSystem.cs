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

    const float STATIC_SHAPE_RATIO = 1 / 4f;

    public void StartSystem()
    {
        int nbOfStaticShape = 0;

        for (int i = 0, length = ECSManager.Instance.Config.allShapesToSpawn.Count; i < length; i++)
        {
            // Create entities and components
            EntityComponent entity = new EntityComponent{ id = (uint)i };
            CircleComponent circleComponent = new CircleComponent{ ShapeConfig = ECSManager.Instance.Config.allShapesToSpawn[i] };
            PositionComponent positionComponent = new PositionComponent{ Position = ECSManager.Instance.Config.allShapesToSpawn[i].initialPos };
            SpeedComponent speedComponent = new SpeedComponent{ Speed = ECSManager.Instance.Config.allShapesToSpawn[i].initialSpeed };
            ColorComponent colorComponent = new ColorComponent();

            
            if (speedComponent.Speed.magnitude < float.Epsilon)
            {
                colorComponent.Color = Color.red;
                nbOfStaticShape++;
            }
            else
            {
                colorComponent.Color = Color.blue;
            }

            World.Instance.AddComponent(entity, circleComponent);
            World.Instance.AddComponent(entity, positionComponent);
            World.Instance.AddComponent(entity, speedComponent);
            World.Instance.AddComponent(entity, colorComponent);

            ECSManager.Instance.CreateShape(entity.id, circleComponent.ShapeConfig);
            ECSManager.Instance.UpdateShapeColor(entity.id, colorComponent.Color);
        }

        int quarterOfShapes = (int)(ECSManager.Instance.Config.allShapesToSpawn.Count * STATIC_SHAPE_RATIO);

        if (nbOfStaticShape < quarterOfShapes)
        {
            int remainingShapesToChange = quarterOfShapes - nbOfStaticShape;
            foreach (KeyValuePair<EntityComponent, SpeedComponent> speed in World.Instance.GetComponentsDict<SpeedComponent>())
            {
                if(speed.Value.Speed.magnitude > float.Epsilon)
                {
                    World.Instance.GetComponent<ColorComponent>(speed.Key).Color = Color.red;
                    speed.Value.Speed = Vector2.zero;
                    ECSManager.Instance.UpdateShapeColor(speed.Key.id, Color.red);

                    remainingShapesToChange--;

                    if (remainingShapesToChange == 0)
                        break;
                }
            }
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
