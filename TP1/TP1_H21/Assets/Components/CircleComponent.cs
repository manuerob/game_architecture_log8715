using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Config;

public class CircleComponent : ICopiableComponent
{
    public ShapeConfig ShapeConfig;

    public object Clone()
    {
        return new CircleComponent { ShapeConfig = ShapeConfig };
    }
}