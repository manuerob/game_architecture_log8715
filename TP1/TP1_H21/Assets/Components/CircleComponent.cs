using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Config;

public class CircleComponent : IComponent
{
    ShapeConfig shapeConfig;
    public ShapeConfig ShapeConfig => shapeConfig;
    public CircleComponent(ShapeConfig _shapeConfig)
    {
        shapeConfig = _shapeConfig;
    }
}
