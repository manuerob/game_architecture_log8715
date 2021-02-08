using System.Collections.Generic;

public class DisplaySystem : ISystem
{
    public string Name => "DisplaySystem";

    public void UpdateSystem()
    {
        foreach (KeyValuePair<EntityComponent, SpeedComponent> speed in World.Instance.GetComponentsDict<SpeedComponent>())
        {
            ECSManager.Instance.UpdateShapeColor(speed.Key.id, World.Instance.GetComponent<ColorComponent>(speed.Key).Color);
            ECSManager.Instance.UpdateShapeSize(speed.Key.id, World.Instance.GetComponent<SizeComponent>(speed.Key).Size);
            ECSManager.Instance.UpdateShapePosition(speed.Key.id, World.Instance.GetComponent<PositionComponent>(speed.Key).Position);
        }
    }
}