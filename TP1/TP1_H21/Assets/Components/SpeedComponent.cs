using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedComponent : EntityComponent
{
    public Vector2 Speed;
    public SpeedComponent(uint _id, Vector2 _speed)
    {
        id = _id;
        Speed = _speed;
    }
}
