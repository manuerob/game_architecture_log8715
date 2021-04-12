using UnityEngine;

public class InputSystem : ISystem
{
    const float speed = 200;

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
                    if (ECSManager.Instance.Config.enableInputPrediction)
                    { 
                        shapeComponent.speed.x = horizontal * speed * Time.deltaTime;
                        shapeComponent.speed.y = vertical * speed * Time.deltaTime;
                        ComponentsManager.Instance.SetComponent<ShapeComponent>(entityID, shapeComponent);
                    }
                    
                    bool hasInputComponent = ComponentsManager.Instance.TryGetComponent(entityID, out InputComponent inputs);
                    if (hasInputComponent)
                    {
                        inputs.horizontal = horizontal;
                        inputs.vertical = vertical;
                    }
                    else 
                    {
                        inputs = new InputComponent()
                        {
                            entityId = entityID.id,
                            horizontal = horizontal,
                            vertical = vertical
                        };
                    }

                    ComponentsManager.Instance.SetComponent<InputComponent>(entityID, inputs);
                }
            });
        }
    }
}