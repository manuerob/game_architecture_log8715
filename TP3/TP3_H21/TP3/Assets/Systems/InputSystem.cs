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

            const float speed = 5;

            ComponentsManager.Instance.ForEach<PlayerComponent, ShapeComponent>((entityID, playerComponent, shapeComponent) => {

                if (ECSManager.Instance.NetworkManager.LocalClientId == playerComponent.playerId) {

                    shapeComponent.speed.x = horizontalMovement * speed * Time.deltaTime;
                    shapeComponent.speed.y = verticalMovement * speed * Time.deltaTime;
                    ComponentsManager.Instance.SetComponent<ShapeComponent>(entityID, shapeComponent);
                }
            });
        }   }
}