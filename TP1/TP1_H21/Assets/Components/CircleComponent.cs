using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Config;

public class CircleComponent : EntityComponent
{
    ShapeConfig shapeConfig;
    public ShapeConfig ShapeConfig => shapeConfig;
    public CircleComponent(uint _id, ShapeConfig _shapeConfig)
    {
        id = _id;
        shapeConfig = _shapeConfig;
    }
}
