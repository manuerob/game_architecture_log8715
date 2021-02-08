using UnityEngine;

public class RestoreStateSystem : ISystem
{
    public string Name => "RestoreStateSystem";

    public void UpdateSystem()
    {
        if (World.Instance.cooldownValue <= float.Epsilon)
        {
            if (Input.GetKeyDown("space"))
            {
                World.Instance.RestoreState();
            }
        }
        else
        {
            if (Input.GetKeyDown("space"))
            {
                Debug.Log("Time remaining (s): " + World.Instance.cooldownValue);
            }
            World.Instance.cooldownValue -= Time.deltaTime;
        }
    }
}