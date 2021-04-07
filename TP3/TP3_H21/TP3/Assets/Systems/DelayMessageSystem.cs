using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DelayMessageSystem : ISystem
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
                    DelayMessage msg = new DelayMessage()
                    {
                        messageID = 0,
                        timeCreated = Utils.SystemTime,
                        entityId = entityID.id,
                        delayMessageId = ComponentsManager.Instance.GetDelayMessageId,
                        senderId = ECSManager.Instance.NetworkManager.LocalClientId
                    };
                    ComponentsManager.Instance.SetComponent<DelayMessage>(entityID, msg);
                    
                    ComponentsManager.Instance.AddToDelayHistory(msg);
                }
            });
        }
        else
        {
            ComponentsManager.Instance.ForEach<PlayerComponent>((entityID, playerComponent) =>
            {
                if (ComponentsManager.Instance.DelayQueueCount > 0
                    && ComponentsManager.Instance.DelayQueueContainsEntity(entityID))
                {
                    DelayMessage clientMsg = ComponentsManager.Instance.GetFromDelayQueue(entityID);

                    DelayMessage responseMsg = new DelayMessage()
                    {
                        messageID = 0,
                        timeCreated = Utils.SystemTime,
                        entityId = clientMsg.entityId,
                        senderId = clientMsg.senderId,
                        delayMessageId = clientMsg.delayMessageId
                    };
                    ComponentsManager.Instance.SetComponent<DelayMessage>(clientMsg.entityId, responseMsg);
                }
            });
        }
    }
}
