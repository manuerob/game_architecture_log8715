using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem : ISystemUpdatablePerEntity
{
    public string Name => "CollisionSystem";

    public static readonly Vector2Int[] WallNormals = new Vector2Int[4]
    {
        new Vector2Int( 0,  1), // Bottom wall
        new Vector2Int( 0, -1), // Top wall
        new Vector2Int(-1,  0), // Right wall
        new Vector2Int( 1,  0), // Left wall
    };

    public void UpdateSystem()
    {
        foreach (KeyValuePair<EntityComponent, SpeedComponent> speed in World.Instance.GetComponentsDict<SpeedComponent>())
        {
            UpdatePerEntity(speed.Key);
        }
    }

    public void UpdatePerEntity(EntityComponent entity)
    {
        // Collide with other circles
        CollideWithCircles(entity);

        // Check wall collisions and rebound if necessary
        CollideWithWalls(entity);
    }

    private void CollideWithCircles(EntityComponent entity)
    {
        if (World.Instance.GetComponentsDict<CanCollideComponent>().ContainsKey(entity))
        {
            foreach (KeyValuePair<EntityComponent, SizeComponent> other in World.Instance.GetComponentsDict<SizeComponent>())
            {
                if (entity.id != other.Key.id
                    && World.Instance.GetComponentsDict<CanCollideComponent>().ContainsKey(other.Key))
                {
                    SizeComponent size = World.Instance.GetComponent<SizeComponent>(entity);
                    SizeComponent size2 = other.Value;
                    PositionComponent position = World.Instance.GetComponent<PositionComponent>(entity);
                    PositionComponent position2 = World.Instance.GetComponent<PositionComponent>(other.Key);

                    if ((position2.Position - position.Position).magnitude < (size.Size + size2.Size) / 2)
                    {
                        ReboundFromCircle(entity);

                        if (World.Instance.GetComponentsDict<SpeedComponent>().ContainsKey(other.Key))
                        {
                            ReboundFromCircle(other.Key);
                        }
                    }
                }
            }
        }
    }

    private void CollideWithWalls(EntityComponent entity)
    {
        PositionComponent position = World.Instance.GetComponent<PositionComponent>(entity);
        SizeComponent size = World.Instance.GetComponent<SizeComponent>(entity);

        for (int i = 0; i < 4; i++)
        {
            Vector2 positionRelativeToWall = position.Position - World.Instance.WallCenters[i];
            Vector2 normal = World.Instance.WallNormals[i];

            float distanceToWall = Vector2.Dot(positionRelativeToWall, normal);

            if (distanceToWall - size.Size / 2f <= 0)
            {
                ReboundFromWall(entity, normal, distanceToWall);
            }
        }
    }

    private void ReboundFromWall(EntityComponent entity, Vector2 normal, float distanceToWall)
    {
        TurnIntoNotGhost(entity);

        PositionComponent position = World.Instance.GetComponent<PositionComponent>(entity);
        SpeedComponent speed = World.Instance.GetComponent<SpeedComponent>(entity);
        SizeComponent size = World.Instance.GetComponent<SizeComponent>(entity);

        // Calculate rebound speed
        speed.Speed = speed.Speed - 2 * normal * Vector2.Dot(speed.Speed, normal);

        // Calculate rebound position
        position.Position += normal * (size.Size / 2 - distanceToWall);
    }

    private void ReboundFromCircle(EntityComponent entity)
    {
        SpeedComponent speed = World.Instance.GetComponent<SpeedComponent>(entity);
        SizeComponent size = World.Instance.GetComponent<SizeComponent>(entity);

        size.Size /= 2;
        speed.Speed *= -1;

        TurnIntoGhost(size.Size, entity);
    }

    private void TurnIntoGhost(float size, EntityComponent entity)
    {
        if (size < ECSManager.Instance.Config.minSize)
        {
            World.Instance.RemoveComponent<CanCollideComponent>(entity);
            World.Instance.GetComponent<ColorComponent>(entity).Color = Color.green;
        }
    }

    private void TurnIntoNotGhost(EntityComponent entity)
    {
        World.Instance.GetComponent<ColorComponent>(entity).Color = Color.blue;
        if (!World.Instance.GetComponentsDict<CanCollideComponent>().ContainsKey(entity))
            World.Instance.AddComponent<CanCollideComponent>(entity, new CanCollideComponent());
        World.Instance.GetComponent<SizeComponent>(entity).Size = World.Instance.GetComponent<CircleComponent>(entity).ShapeConfig.size;
    }
}