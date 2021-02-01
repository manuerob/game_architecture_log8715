using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem : ISystem
{
    public static readonly Vector2Int[] WallNormals = new Vector2Int[4]
    {
        new Vector2Int( 0,  1), // Bottom wall
        new Vector2Int( 0, -1), // Top wall
        new Vector2Int(-1,  0), // Right wall
        new Vector2Int( 1,  0), // Left wall
    };

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


            if (CheckWallCollision(position.Position, size.Size / 2f, out Vector2 normal))
            {
                ReboundFromWall(speed.Key, normal);
            }
        }
    }

    // Check for edgecase (coin)
    private bool CheckWallCollision(Vector2 position, float radius, out Vector2 normal)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 positionRelativeToWall = position - World.Instance.WallCenters[i];
            normal = World.Instance.WallNormals[i];

            float distanceToWall = Vector2.Dot(positionRelativeToWall, normal);

            if (distanceToWall - radius <= 0)
            {
                return true;
            }
        }

        normal = Vector2.zero;
        return false;
    }

    private void ReboundFromWall(EntityComponent entity, Vector2 normal)
    {

        float previousSize = World.Instance.GetComponent<SizeComponent>(entity).Size;
        Vector2 previousSpeed = World.Instance.GetComponent<SpeedComponent>(entity).Speed;



        World.Instance.GetComponent<SpeedComponent>(entity).Speed = previousSpeed - 2 * normal * Vector2.Dot(previousSpeed, normal);
        World.Instance.GetComponent<ColorComponent>(entity).Color = Color.blue;

        if (!World.Instance.GetComponentsDict<CanCollideComponent>().ContainsKey(entity))
            World.Instance.AddComponent<CanCollideComponent>(entity, new CanCollideComponent());
        World.Instance.GetComponent<SizeComponent>(entity).Size = World.Instance.GetComponent<CircleComponent>(entity).ShapeConfig.size;

        ECSManager.Instance.UpdateShapeColor(entity.id, World.Instance.GetComponent<ColorComponent>(entity).Color);
        ECSManager.Instance.UpdateShapeSize(entity.id, World.Instance.GetComponent<SizeComponent>(entity).Size);

        float deltaSize = World.Instance.GetComponent<SizeComponent>(entity).Size - previousSize;



        PositionComponent position = World.Instance.GetComponent<PositionComponent>(entity);

        position.Position += normal * deltaSize / 2 + (Mathf.Sign(deltaSize) * float.Epsilon * normal);// - previousSpeed * Time.deltaTime; // remplacer par le vrai delta position?
        ECSManager.Instance.UpdateShapePosition(entity.id, position.Position);


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