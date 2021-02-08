using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimesToRepeatSimulationComponent : ICopiableComponent
{
    public int TimesToRepeat;

    public object Clone()
    {
        return new TimesToRepeatSimulationComponent { TimesToRepeat = TimesToRepeat };
    }
}
