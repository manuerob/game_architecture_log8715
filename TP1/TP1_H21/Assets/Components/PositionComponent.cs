using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionComponent : ICopiableComponent
{
    public Vector2 Position;

    public object Clone()
    {
        return new PositionComponent { Position = Position };
    }
}

