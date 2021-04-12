using UnityEngine;

public class ExtrapolationSystem : ISystem
{
    public string Name => GetType().Name;

    public void UpdateSystem()
    {
        if (ECSManager.Instance.NetworkManager.IsClient && !ECSManager.Instance.NetworkManager.IsServer)
        {
            // calculate the number of frames to compute
            ulong rttMilliseconds = ECSManager.Instance.GetCurrentRtt();
            

            int amoutOfFramesToExtrapolate = (int)(((rttMilliseconds / 2f) / 1000) / Time.deltaTime);

            Debug.Log("Frames to extrapolate = " + amoutOfFramesToExtrapolate);

            for (int i = 0; i < amoutOfFramesToExtrapolate; i++)
            {
                ECSManager.Instance.UpdateExtrapolatedSystems();
                ComponentsManager.Instance.ClearComponents<CollisionEventComponent>();
            }
        }
    }
}