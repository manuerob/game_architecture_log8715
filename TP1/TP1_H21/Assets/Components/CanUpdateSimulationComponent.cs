using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanUpdateSimulationComponent : ICopiableComponent
{
    public object Clone()
    {
        return new CanUpdateSimulationComponent();
    }
}
