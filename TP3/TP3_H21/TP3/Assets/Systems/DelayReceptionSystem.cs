using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DelayReceptionSystem : ISystem
{
    public string Name
    {
        get
        {
            return GetType().Name;
        }
    }

    public void UpdateSystem()
    {
        if (ECSManager.Instance.NetworkManager.IsClient && !ECSManager.Instance.NetworkManager.IsServer)
        {
            ComponentsManager.Instance.ForEach<PlayerComponent>((entityID, playerComponent) =>
            {
                if (ECSManager.Instance.NetworkManager.LocalClientId == playerComponent.playerId)
                {
                    if (ComponentsManager.Instance.DelayQueueCount > 0)
                    {
                        DelayMessage responseMsg = ComponentsManager.Instance.GetFromDelayQueue();

                        var history = ComponentsManager.Instance.GetDelayHistory();
                        int matchIndex = history.FindIndex(x => x.delayMessageId == responseMsg.delayMessageId);

                        if (matchIndex > 0)
                        {
                            history.RemoveRange(0, matchIndex);

                            DelayMessage correspondingLocalMsg = ComponentsManager.Instance.GetFirstFromDelayHistory();

                            ComponentsManager.Instance.delayMs = responseMsg.timeCreated - correspondingLocalMsg.timeCreated;

                            ComponentsManager.Instance.RemoveFirstFromDelayHistory();
                            
                        }
                    }
                }
            });
        }
    }
}
