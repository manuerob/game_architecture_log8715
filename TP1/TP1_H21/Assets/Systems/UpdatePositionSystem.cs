using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePositionSystem : ISystem
{
    public string Name => "UpdatePositionSystem";

    public void UpdateSystem()
    {
        foreach (PositionComponent positionComponent in World.Instance.GetComponentsList<PositionComponent>())
        {
            uint id = positionComponent.id;

            positionComponent.Position += World.Instance.GetComponent<SpeedComponent>((int)id).Speed * Time.deltaTime;

            ECSManager.Instance.UpdateShapePosition(id, positionComponent.Position);
        }
    }
}
