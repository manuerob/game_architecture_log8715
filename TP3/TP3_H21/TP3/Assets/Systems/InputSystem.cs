using UnityEngine;

public class InputSystem : ISystem
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
        if(ECSManager.Instance.NetworkManager.IsClient && !ECSManager.Instance.NetworkManager.IsServer)
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");

            ComponentsManager.Instance.ForEach<PlayerComponent>((entityID, playerComponent) =>
            {
                if (ECSManager.Instance.NetworkManager.LocalClientId == playerComponent.playerId)
                {
                    InputMessage msg = new InputMessage()
                    {
                        messageID = 0,
                        timeCreated = Utils.SystemTime,
                        entityId = entityID.id,
                        horizontal = horizontalMovement,
                        vertical = verticalMovement
                    };
                    ComponentsManager.Instance.SetComponent<InputMessage>(entityID, msg);
                }
            });
        }
        else
        {
            ComponentsManager.Instance.ForEach<ShapeComponent, InputMessage>((entityID, shapeComponent, inputMessage) =>
            {
                const float speed = 5;
                shapeComponent.speed.x = inputMessage.horizontal * speed * Time.deltaTime;
                shapeComponent.speed.y = inputMessage.vertical * speed * Time.deltaTime;
                ComponentsManager.Instance.SetComponent<ShapeComponent>(entityID, shapeComponent);
            });

            ComponentsManager.Instance.ClearComponents<InputMessage>();
        }
    }
}