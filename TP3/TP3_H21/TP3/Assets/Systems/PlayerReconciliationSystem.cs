using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerReconciliationSystem : ISystem
{
    const float PLAYER_RECONCILIATION_THRESHOLD = 0.05f;

    public string Name
    {
        get
        {
            return GetType().Name;
        }
    }

    public bool IsLastInputSentZero()
    {
        InputMessage lastMessage = ComponentsManager.Instance.GetLastFromInputHistory();
        return lastMessage.horizontal == 0 && lastMessage.vertical == 0;
    }

    public void UpdateSystem()
    {
        if (ECSManager.Instance.NetworkManager.IsClient && !ECSManager.Instance.NetworkManager.IsServer)
        {
            ComponentsManager.Instance.ForEach<PlayerComponent, ShapeComponent, InputComponent>((entityID, playerComponent, shapeComponent, inputComponent) =>
            {
                if (ComponentsManager.Instance.InputHistoryCount > 0)
                {
                    if (IsLastInputSentZero())
                    {
                        if (Mathf.Abs(inputComponent.vertical) > float.Epsilon || Mathf.Abs(inputComponent.horizontal) > float.Epsilon)
                        { 
                            TryReconciliation(entityID, playerComponent, shapeComponent, inputComponent);
                        }
                    }
                    else 
                    {
                        TryReconciliation(entityID, playerComponent, shapeComponent, inputComponent);
                    }
                }
                else 
                {
                    TryReconciliation(entityID, playerComponent, shapeComponent, inputComponent);
                }
            });
        }
    }

    public void TryReconciliation(EntityComponent entityID, PlayerComponent playerComponent, ShapeComponent shapeComponent, InputComponent inputComponent)
    {
        if (ECSManager.Instance.NetworkManager.LocalClientId == playerComponent.playerId)
        {
            if (ComponentsManager.Instance.InputQueueCount > 0)
            {
                InputMessage responseMsg = ComponentsManager.Instance.GetFromInputQueue();
                
                var history = ComponentsManager.Instance.GetInputHistory();
                int matchIndex = history.FindIndex(x => x.inputId == responseMsg.inputId);

                if (matchIndex > 0)
                {
                    history.RemoveRange(0, matchIndex);

                    InputMessage correspondingLocalMsg = ComponentsManager.Instance.GetFirstFromInputHistory();

                    if ((responseMsg.pos - correspondingLocalMsg.pos).magnitude > PLAYER_RECONCILIATION_THRESHOLD)
                    {
                        while (ComponentsManager.Instance.InputQueueCount > 0)
                        {
                            Debug.Log("The history does not match the server. Must Reconcilitate.");
                            shapeComponent.pos = responseMsg.pos;
                            foreach (var input in history)
                            {
                                shapeComponent.speed.x = input.horizontal * 200 * Time.deltaTime;
                                shapeComponent.speed.y = input.vertical * 200 * Time.deltaTime;
                                shapeComponent.pos = PositionUpdateSystem.GetNewPosition(shapeComponent.pos, shapeComponent.speed);
                            }
                            responseMsg = ComponentsManager.Instance.GetFromInputQueue();
                            ComponentsManager.Instance.RemoveFirstFromInputHistory();
                        }
                        ComponentsManager.Instance.SetComponent<ShapeComponent>(entityID, shapeComponent);
                    }
                    else
                    {
                        ComponentsManager.Instance.RemoveFirstFromInputHistory();
                    }
                }
                

                // On vient de recevoir le plus vieux dans l'historique en retour du serveur.
                // On sait le temps quand le serveur l'a créé et on sait le temps quand nous on l'a créé
                // La différence de temps est le délai entre le serveur et le client.

                // On peut set un component pour contenir ce temps là et quand on extrapole on
                //   doit deviner selon le serveur les objets sont où. Quand un objet bouge, 
                //   on doit add le décalage selon la vitesse.

                // (Problème: on est conscient du décalage seulement lorsque l'on bouge...
                //   Autrement, le délai est celui de la dernière fois qu'on a bougé.
                //   Il faudrait faire un type de message comme le input mais qu'on envoie en 
                //   continue entre le serveur et le client pour toujours avoir le délai, 
                //   pour pas que la valeur connue du délai dépende d'une feature.)
            }
        }
    }
}
