using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedComponent : IComponent
{
    public Vector2 Speed;
    public SpeedComponent(Vector2 _speed)
    {
        Speed = _speed;
    }
}
