using UnityEngine;

public class NetworkMessageSystem : ISystem
{
    public string Name
    {
        get
        {
            return GetType().Name;
        }
    }

    // In charge of sending all messages pending sending
    public void UpdateSystem()
    {
        bool messagingInfoFound = ComponentsManager.Instance.TryGetComponent(new EntityComponent(0), out MessagingInfo messagingInfo);

        if (!messagingInfoFound)
        {
            messagingInfo = new MessagingInfo() { currentMessageId = 0 };
        }

        if (ECSManager.Instance.NetworkManager.isServer)
        {
            ComponentsManager.Instance.ForEach<ReplicationMessage>((entityID, msg) =>
            {
                msg.messageID = messagingInfo.currentMessageId++;
                ECSManager.Instance.NetworkManager.SendReplicationMessage(msg);
            });

            ComponentsManager.Instance.ForEach<InputMessage>((entityID, msg) => {
                msg.messageID = messagingInfo.currentMessageId++;
                ECSManager.Instance.NetworkManager.SendInputMessage(msg, false);
            });

            ComponentsManager.Instance.ForEach<DelayMessage>((entityID, msg) => {
                msg.messageID = messagingInfo.currentMessageId++;
                ECSManager.Instance.NetworkManager.SendDelayMessage(msg, false);
            });
        }

        if (ECSManager.Instance.NetworkManager.isClient)
        {
            // TODO
            ComponentsManager.Instance.ForEach<InputMessage>((entityID, msg) => {
                msg.messageID = messagingInfo.currentMessageId++;
                ECSManager.Instance.NetworkManager.SendInputMessage(msg, true);
            });

            ComponentsManager.Instance.ForEach<DelayMessage>((entityID, msg) => {
                msg.messageID = messagingInfo.currentMessageId++;
                ECSManager.Instance.NetworkManager.SendDelayMessage(msg, true);
            });

        }

        ComponentsManager.Instance.SetComponent<MessagingInfo>(new EntityComponent(0), messagingInfo);
    }
}