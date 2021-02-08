using System.Collections.Generic;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        List<ISystem> toRegister = new List<ISystem>();

        // AJOUTEZ VOS SYSTEMS ICI
        toRegister.Add(new InitializationSystem());
        toRegister.AddRange(World.Instance.simulationSystems);
        toRegister.Add(new RepeatSimulationSystem());
        toRegister.Add(new SaveStateSystem());
        toRegister.Add(new RestoreStateSystem());
        toRegister.Add(new DisplaySystem());

        return toRegister;
    }
}