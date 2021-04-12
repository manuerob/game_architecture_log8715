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
        toRegister.Add(new InputSystem()); // Récupère les inputs, set la nouvelle vitesse
        toRegister.Add(new BounceBackSystem());
        toRegister.Add(new PositionUpdateSystem());
        toRegister.Add(new PlayerMovementSystem());
        toRegister.Add(new InputMessageSystem());   // Récupère le composant d'inputs, crée le message avec la position calcuée par le client
        toRegister.Add(new PlayerReconciliationSystem());
        toRegister.Add(new ReplicationSystem());
        toRegister.Add(new ExtrapolationSystem());
        toRegister.Add(new NetworkMessageSystem());
        toRegister.Add(new ClearEndOfFrameComponentsSystem());
        toRegister.Add(new DisplayShapePositionSystem());


        // EXTRAPOLATION:
        // Split PositionUpdateSystem et PlayerMovementSystem (Bon choix de maintenabilité. Le déplacement des joueurs risque de devoir être différent des autres déplacements)
        // Add tous les systèmes de simulation npc à une liste de extrapolated systems 
        //     (aucun input, input message, network ou déplacement de joueur. Purement simulation client)
        // Dans ReplicationSystem (à renommer pour ExtrapolationSystem?) quand on reçoit
        //     le replication message: Appliquer les valeurs, run la liste de systèmes (rtt/deltatime) fois. 

        // Est-ce qu'on veut vraiment une condition pour vérifier si on doit extrapoler? (À faire tout le
        //     temps, parce qu'on a dit qu'on extrapole les autres joueurs. Ils peuvent changer de direction
        //     n'importe quand. Comme c'est un "jeu multijoueur", il y aurait toujours de l'extrapolation à
        //     refaire à moins que les joueurs ne bougent tous pas ou qu'ils bougent tous sans changer leur
        //     direction. Donc le ralentissement causé serait la norme. Les gains faits par le if seraient
        //     faits seulement dans de rares cas ou quand un joueur est seul sur le serveur.

        return toRegister;
    }

    public static List<ISystem> GetListOfExtrapolatedSystems()
    {
        List<ISystem> toRegister = new List<ISystem>();

        toRegister.Add(new WallCollisionDetectionSystem());
        toRegister.Add(new CircleCollisionDetectionSystem());
        toRegister.Add(new BounceBackSystem());
        toRegister.Add(new PositionUpdateSystem());
        //toRegister.Add(new ClearEndOfFrameComponentsSystem());

        return toRegister;
    }
}