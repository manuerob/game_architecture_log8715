using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        List<ISystem> toRegister = new List<ISystem>();
        // Add your systems
        toRegister.Add(new SpawnSystem());
        toRegister.Add(new WallCollisionDetectionSystem());
        toRegister.Add(new CircleCollisionDetectionSystem());
        toRegister.Add(new BounceBackSystem());
        toRegister.Add(new InputSystem()); // Récupère les inputs, set la nouvelle vitesse
        toRegister.Add(new PositionUpdateSystem());
        toRegister.Add(new InputMessageSystem());   // Récupère le composant d'inputs, crée le message avec la position calcuée par le client
        toRegister.Add(new PlayerReconciliationSystem());
        toRegister.Add(new ReplicationSystem());
        toRegister.Add(new NetworkMessageSystem());
        toRegister.Add(new ClearEndOfFrameComponentsSystem());
        toRegister.Add(new DisplayShapePositionSystem());

        return toRegister;
    }
}