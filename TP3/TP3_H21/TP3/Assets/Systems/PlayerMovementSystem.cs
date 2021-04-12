using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementSystem : ISystem {
    public string Name => GetType().Name;

    public void UpdateSystem()
    {
        if (ECSManager.Instance.NetworkManager.IsClient && !ECSManager.Instance.NetworkManager.IsServer)
        {
            ComponentsManager.Instance.ForEach<PlayerComponent, ShapeComponent>((entityID, playerMovement, shapeComponent) =>
            {
                shapeComponent.pos += shapeComponent.speed * Time.deltaTime;
                ComponentsManager.Instance.SetComponent<ShapeComponent>(entityID, shapeComponent);
            });
        }
    }
}

