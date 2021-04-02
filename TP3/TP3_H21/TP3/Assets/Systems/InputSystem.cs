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
                if (ECSManager.Instance.NetworkManager.LocalClientId == playerComponent.playerId
                    && (Mathf.Abs(horizontal) > float.Epsilon || Mathf.Abs(vertical) > float.Epsilon))
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

                    if (ComponentsManager.Instance.InputQueueCount > 0)
                    {
                        InputMessage responseMsg = ComponentsManager.Instance.GetFromInputQueue();
                        InputMessage correspondingLocalMsg = ComponentsManager.Instance.GetFirstFromInputHistory();

                        if ((responseMsg.pos - correspondingLocalMsg.pos).magnitude > (float.Epsilon * 10))
                        {
                            Debug.LogWarning("The history does not match the server. Must Reconcilitate.");
                        }

                        ComponentsManager.Instance.RemoveFirstFromInputHistory();
                    }
                }
            });
        }
        else
        {
            while (ComponentsManager.Instance.InputQueueCount > 0)
            {
                InputMessage clientMsg = ComponentsManager.Instance.GetFromInputQueue();

                ShapeComponent component = ComponentsManager.Instance.GetComponent<ShapeComponent>(clientMsg.entityId);
                component.pos.x += clientMsg.horizontal * speed * Time.deltaTime;
                component.pos.y += clientMsg.vertical * speed * Time.deltaTime;
                ComponentsManager.Instance.SetComponent<ShapeComponent>(clientMsg.entityId, component);

                InputMessage responseMsg = new InputMessage()
                {
                    messageID = 0,
                    timeCreated = Utils.SystemTime,
                    entityId = clientMsg.entityId,
                    horizontal = clientMsg.horizontal, // Pas nécessaire. À enlever?
                    vertical = clientMsg.vertical,
                    pos = component.pos // Envoi de la position attendue suite à l'input
                };
                ComponentsManager.Instance.SetComponent<InputMessage>(clientMsg.entityId, responseMsg);
            }
        }
    }
}