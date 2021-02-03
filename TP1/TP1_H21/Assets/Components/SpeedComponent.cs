using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedComponent : ICopiableComponent
{
    public Vector2 Speed;

    public object Clone()
    {
        return new SpeedComponent{ Speed = Speed };
    }
}
