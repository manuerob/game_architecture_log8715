using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreStateSystem : ISystem
{
    public string Name => "RestoreStateSystem";

    public void UpdateSystem()
    {
        if(World.Instance.coolDownValue <= float.Epsilon)
        {
            if (Input.GetKeyDown("space"))
            {
                World.Instance.RestoreState();
                World.Instance.coolDownValue = World.Instance.coolDownInitialValue;

            }
        }
        else
        {
            if (Input.GetKeyDown("space"))
            {
                Debug.Log("Time remaining (s): " + World.Instance.coolDownValue);

            }
            World.Instance.coolDownValue -= Time.deltaTime;
        }
    }
}