using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimesToExecuteSimulationComponent : ICopiableComponent
{
    public int TimesToExecute;

    public object Clone()
    {
        return new TimesToExecuteSimulationComponent { TimesToExecute = TimesToExecute };
    }
}
