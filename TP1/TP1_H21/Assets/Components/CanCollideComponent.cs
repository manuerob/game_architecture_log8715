using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanCollideComponent : ICopiableComponent
{
    public object Clone()
    {
        return new CanCollideComponent();
    }
}
