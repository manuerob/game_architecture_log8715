using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrapolationSystem : ISystem {
    public string Name
    {
        get
        {
            return this.GetType().Name;
        }
    }

    public void UpdateSystem()
    {
        ComponentsManager.Instance.ForEach<ShapeComponent>((entityID, shapeComponent) => {



            shapeComponent.pos += shapeComponent.speed * (ComponentsManager.Instance.delayMs / 1000f);
            ComponentsManager.Instance.SetComponent<ShapeComponent>(entityID, shapeComponent);
        });
    }
}

