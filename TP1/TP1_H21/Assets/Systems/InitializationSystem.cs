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
        InitializeCircleEntities();

        InitializeWorldScreenPositions();

    }

    private void InitializeCircleEntities()
    {
        int nbOfStaticShape = 0;

        for (int i = 0, length = ECSManager.Instance.Config.allShapesToSpawn.Count; i < length; i++)
        {
            // Create entities and components
            EntityComponent entity = new EntityComponent { id = World.Instance.GetNextId() };
            CircleComponent circleComponent = new CircleComponent { ShapeConfig = ECSManager.Instance.Config.allShapesToSpawn[i] };
            PositionComponent positionComponent = new PositionComponent { Position = ECSManager.Instance.Config.allShapesToSpawn[i].initialPos };
            SizeComponent sizeComponent = new SizeComponent { Size = ECSManager.Instance.Config.allShapesToSpawn[i].size };
            ColorComponent colorComponent = new ColorComponent();

            if (circleComponent.ShapeConfig.initialSpeed.magnitude < float.Epsilon)
            {
                colorComponent.Color = Color.red;
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

            if (sizeComponent.Size > ECSManager.Instance.Config.minSize)
            {
                World.Instance.AddComponent(entity, new CanCollideComponent());
            }
            else if (colorComponent.Color != Color.red)
            {
                colorComponent.Color = Color.green;
            }

            ECSManager.Instance.CreateShape(entity.id, circleComponent.ShapeConfig);
            ECSManager.Instance.UpdateShapeColor(entity.id, colorComponent.Color);
            ECSManager.Instance.UpdateShapePosition(entity.id, positionComponent.Position);
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

                remainingShapesToChange--;

                if (remainingShapesToChange == 0)
                    break;
            }

            foreach (KeyValuePair<EntityComponent, SpeedComponent> component in speedsToRemove)
            {
                World.Instance.RemoveComponent<SpeedComponent>(component.Key);
            }


        }

    }

    private void InitializeWorldScreenPositions()
    {
        World.Instance.WallCenters[0] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, 0, 0));
        World.Instance.WallCenters[1] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height, 0));
        World.Instance.WallCenters[2] = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, 0));
        World.Instance.WallCenters[3] = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, 0));
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
