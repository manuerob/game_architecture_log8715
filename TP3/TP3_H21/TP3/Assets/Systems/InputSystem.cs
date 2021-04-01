using UnityEngine;

public class InputSystem : ISystem
{
    const float speed = 20;

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
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            ComponentsManager.Instance.ForEach<PlayerComponent, ShapeComponent>((entityID, playerComponent, shapeComponent) =>
            {
                if (ECSManager.Instance.NetworkManager.LocalClientId == playerComponent.playerId)
                {
                    // Prédiction client du déplacement
                    shapeComponent.pos.x += horizontal * speed * Time.deltaTime;
                    shapeComponent.pos.y += vertical * speed * Time.deltaTime;
                    ComponentsManager.Instance.SetComponent<ShapeComponent>(entityID, shapeComponent);

                    InputMessage msg = new InputMessage()
                    {
                        messageID = 0,
                        timeCreated = Utils.SystemTime,
                        entityId = entityID.id,
                        horizontal = horizontal, // Envoi de l'input fait
                        vertical = vertical,
                        pos = shapeComponent.pos // Envoi de la position attendue suite à l'input
                    };
                    ComponentsManager.Instance.SetComponent<InputMessage>(entityID, msg);

                    // Ajout de msg dans l'historique
                    ComponentsManager.Instance.AddToInputHistory(msg);
                }
            });
        }
        else
        {
            if (ComponentsManager.Instance.InputQueueCount > 0)
            {
                InputMessage msg = ComponentsManager.Instance.GetFromInputQueue();

                ShapeComponent component = ComponentsManager.Instance.GetComponent<ShapeComponent>(msg.entityId);
                component.pos.x += msg.horizontal * speed * Time.deltaTime;
                component.pos.y += msg.vertical * speed * Time.deltaTime;
                ComponentsManager.Instance.SetComponent<ShapeComponent>(msg.entityId, component);

            }
        }
    }
}