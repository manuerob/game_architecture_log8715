using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class InputMessageSystem : ISystem
{
    const float playerSpeed = 200;

    public string Name
    {
        get
        {
            return GetType().Name;
        }
    }

    public bool IsLastInputSentZero()
    {
        InputMessage lastMessage = ComponentsManager.Instance.GetLastFromInputHistory();
        return lastMessage.horizontal == 0 && lastMessage.vertical == 0;
    }

    public void UpdateSystem()
    {
        if (ECSManager.Instance.NetworkManager.IsClient && !ECSManager.Instance.NetworkManager.IsServer)
        {
            ComponentsManager.Instance.ForEach<PlayerComponent, ShapeComponent, InputComponent>((entityID, playerComponent, shapeComponent, inputComponent) =>
            {
                if (ComponentsManager.Instance.InputHistoryCount > 0)
                {
                    if (IsLastInputSentZero())
                    {
                        if (Mathf.Abs(inputComponent.vertical) > float.Epsilon || Mathf.Abs(inputComponent.horizontal) > float.Epsilon)
                        { 
                            SendInputMessage(entityID, playerComponent, shapeComponent, inputComponent);
                        }
                    }
                    else 
                    {
                        SendInputMessage(entityID, playerComponent, shapeComponent, inputComponent);
                    }
                }
                else 
                {
                    SendInputMessage(entityID, playerComponent, shapeComponent, inputComponent);
                }
            });
        }
        else
        {
            while (ComponentsManager.Instance.InputQueueCount > 0)
            {
                InputMessage clientMsg = ComponentsManager.Instance.GetFromInputQueue();

                ShapeComponent component = ComponentsManager.Instance.GetComponent<ShapeComponent>(clientMsg.entityId);
                component.speed.x = clientMsg.horizontal * playerSpeed * Time.deltaTime;
                component.speed.y = clientMsg.vertical * playerSpeed * Time.deltaTime;
                ComponentsManager.Instance.SetComponent<ShapeComponent>(clientMsg.entityId, component);

                InputMessage responseMsg = new InputMessage()
                {
                    messageID = 0,
                    timeCreated = Utils.SystemTime,
                    entityId = clientMsg.entityId,
                    senderId = clientMsg.senderId,
                    inputId = clientMsg.inputId,
                    horizontal = clientMsg.horizontal, // Pas nécessaire. À enlever?
                    vertical = clientMsg.vertical,
                    speed = component.speed, // Envoi de la vitesse attendue suite à l'input
                    pos = component.pos // Envoi de la position attendue suite à l'input
                };
                ComponentsManager.Instance.SetComponent<InputMessage>(clientMsg.entityId, responseMsg);
            }
        }
    }

    public void SendInputMessage(EntityComponent entityID, PlayerComponent playerComponent, ShapeComponent shapeComponent, InputComponent inputComponent)
    {
        if (ECSManager.Instance.NetworkManager.LocalClientId == playerComponent.playerId)
        {
            InputMessage msg = new InputMessage()
            {
                messageID = 0,
                timeCreated = Utils.SystemTime,
                entityId = entityID.id,
                inputId = ComponentsManager.Instance.GetInputId,
                senderId = ECSManager.Instance.NetworkManager.LocalClientId,
                horizontal = inputComponent.horizontal, // Envoi de l'input fait
                vertical = inputComponent.vertical,
                speed = shapeComponent.speed, // Envoi de la vitesse attendue suite à l'input
                pos = shapeComponent.pos // Envoi de la position attendue suite à l'input
            };
            ComponentsManager.Instance.SetComponent<InputMessage>(entityID, msg);

            // Ajout de msg dans l'historique 
            ComponentsManager.Instance.AddToInputHistory(msg);
        }
    }
}
