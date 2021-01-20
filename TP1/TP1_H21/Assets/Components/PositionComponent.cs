using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionComponent : EntityComponent
{
    public Vector2 Position;
    public PositionComponent(uint _id, Vector2 _position)
    {
        id = _id;
        Position = _position;
    }
}
