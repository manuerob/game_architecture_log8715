using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeComponent : ICopiableComponent
{
    public float Size;

    public object Clone()
    {
        return new SizeComponent { Size = Size };
    }
}
