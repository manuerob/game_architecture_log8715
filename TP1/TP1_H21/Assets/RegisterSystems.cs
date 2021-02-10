using System.Collections.Generic;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        List<ISystem> toRegister = new List<ISystem>();

        // AJOUTEZ VOS SYSTEMS ICI
        toRegister.Add(new InitializationSystem());

        for(int i = 0; i < World.Instance.timescale; i++)
        {
            toRegister.Add(new UpdatePositionSystem());
            toRegister.Add(new CollisionSystem());
            toRegister.Add(new UpdateTimesToExecuteSimulationSystem());
        }

        toRegister.Add(new SaveStateSystem());
        toRegister.Add(new RestoreStateSystem());
        toRegister.Add(new DisplaySystem());

        return toRegister;
    }
}