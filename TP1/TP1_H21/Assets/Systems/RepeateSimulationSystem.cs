using System.Collections.Generic;

public class RepeatSimulationSystem : ISystem
{
    public string Name => "RepeatSimulationSystem";
    public void UpdateSystem()
    {
        for (int i = 0; i < World.Instance.timescale; i++)
        {
            foreach (KeyValuePair<EntityComponent, TimesToRepeatSimulationComponent> timesToRepeat in World.Instance.GetComponentsDict<TimesToRepeatSimulationComponent>())
            {
                foreach (ISystemUpdatablePerEntity system in World.Instance.simulationSystems)
                {
                    if (timesToRepeat.Value.TimesToRepeat > 0)
                    {
                        system.UpdatePerEntity(timesToRepeat.Key);
                    }
                }    
            }
        }
    }
}
