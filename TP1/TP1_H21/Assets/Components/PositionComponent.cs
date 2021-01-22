using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionComponent : IComponent
{
    public Vector2 Position;
    public PositionComponent(Vector2 _position)
    {
        Position = _position;
    }
}
