using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePositionSystem : ISystem
{
    public string Name => "UpdatePositionSystem";

    // petit hack temporaire, remplacer par singleton
    bool isStarting = true;

    public void StartSystem()
    {
        for (int i = 0, length = ECSManager.Instance.Config.allShapesToSpawn.Count; i < length; i++)
        {
            // Create entities and components
            EntityManager.Instance.AddPositionComponent(new PositionComponent((uint)i, ECSManager.Instance.Config.allShapesToSpawn[i].initialPos));
            EntityManager.Instance.AddSpeedComponent(new SpeedComponent((uint)i, ECSManager.Instance.Config.allShapesToSpawn[i].initialSpeed));
        }
    }

    public void UpdateSystem()
    {
        if (isStarting)
        {
            isStarting = false;
            StartSystem();
        }

        foreach (PositionComponent positionComponent in EntityManager.Instance.PositionComponents)
        {
            uint id = positionComponent.id;

            positionComponent.Position += EntityManager.Instance.SpeedComponents[(int)id].Speed * Time.deltaTime;

            ECSManager.Instance.UpdateShapePosition(id, positionComponent.Position);
        }
    }

}
