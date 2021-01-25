using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Config;

/// <summary>
/// Retirer speed component si il est a zero
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
            SizeComponent sizeComponent = new SizeComponent { Size = ECSManager.Instance.Config.allShapesToSpawn[i].size };
            ColorComponent colorComponent = new ColorComponent();

            if (circleComponent.ShapeConfig.initialSpeed.magnitude < float.Epsilon)
            {
                colorComponent.Color = Color.red;
                World.Instance.AddComponent(entity, new IsStaticComponent());
                nbOfStaticShape++;
            }
            else
            {
                SpeedComponent speedComponent = new SpeedComponent { Speed = ECSManager.Instance.Config.allShapesToSpawn[i].initialSpeed };
                World.Instance.AddComponent(entity, speedComponent);

                colorComponent.Color = Color.blue;
            }

            World.Instance.AddComponent(entity, circleComponent);
            World.Instance.AddComponent(entity, positionComponent);
            World.Instance.AddComponent(entity, colorComponent);
            World.Instance.AddComponent(entity, sizeComponent);

            ECSManager.Instance.CreateShape(entity.id, circleComponent.ShapeConfig);
            ECSManager.Instance.UpdateShapeColor(entity.id, colorComponent.Color);
        }

        int quarterOfShapes = (int)(ECSManager.Instance.Config.allShapesToSpawn.Count * STATIC_SHAPE_RATIO);

        if (nbOfStaticShape < quarterOfShapes)
        {
            int remainingShapesToChange = quarterOfShapes - nbOfStaticShape;

            List<KeyValuePair<EntityComponent, SpeedComponent>> speedsToRemove = new List<KeyValuePair<EntityComponent, SpeedComponent>>();
            
            Dictionary<EntityComponent, SpeedComponent> speedDictionnary = World.Instance.GetComponentsDict<SpeedComponent>();

            foreach (KeyValuePair<EntityComponent, SpeedComponent> speed in speedDictionnary)
            {
                World.Instance.GetComponent<ColorComponent>(speed.Key).Color = Color.red;

                speedsToRemove.Add(speed);

                ECSManager.Instance.UpdateShapeColor(speed.Key.id, Color.red);
                World.Instance.AddComponent(speed.Key, new IsStaticComponent());

                remainingShapesToChange--;

                if (remainingShapesToChange == 0)
                    break;
            }

            foreach (KeyValuePair<EntityComponent, SpeedComponent> component in speedsToRemove)
            {
                speedDictionnary.Remove(component.Key);
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
