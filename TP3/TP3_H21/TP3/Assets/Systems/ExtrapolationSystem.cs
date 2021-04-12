using UnityEngine;

public class ExtrapolationSystem : ISystem
{
    public string Name => GetType().Name;

    public void UpdateSystem()
    {
        if (ECSManager.Instance.NetworkManager.IsClient && !ECSManager.Instance.NetworkManager.IsServer && ECSManager.Instance.Config.enableDeadReckoning)
        {
            // calculate the number of frames to compute
            ulong rttMilliseconds = ECSManager.Instance.GetCurrentRtt();
            Debug.Log("RTT = " + rttMilliseconds);
            int amoutOfFramesToExtrapolate = (int)(((rttMilliseconds) / 1000f) / Time.deltaTime);

            Debug.Log("Frames to extrapolate = " + amoutOfFramesToExtrapolate);

            for (int i = 0; i < amoutOfFramesToExtrapolate; i++)
            {
                ECSManager.Instance.UpdateExtrapolatedSystems();
                ComponentsManager.Instance.ClearComponents<CollisionEventComponent>();
            }
        }
    }
}