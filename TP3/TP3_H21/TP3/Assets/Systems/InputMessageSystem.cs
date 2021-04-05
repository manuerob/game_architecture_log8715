using UnityEngine;
using System.Linq;
using System.Collections.Generic;

// Il faudrait que ce système prenne les inputs, update la vitesse, ajoute un composant d'inputs (vertical et horizontal)
// Ensuite, après un système qui met à jour la position, on aurait un autre système qui récupère le composant d'input pour créer le message
public class InputMessageSystem : ISystem
{
    const float playerSpeed = 200;
    const float PLAYER_RECONCILIATION_THRESHOLD = 0.5f;

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
                            SendInputMessage(entityID, playerComponent, shapeComponent, inputComponent);
                        }
                    }
                    else 
                    {
                        SendInputMessage(entityID, playerComponent, shapeComponent, inputComponent);
                    }
                }
                else 
                {
                    SendInputMessage(entityID, playerComponent, shapeComponent, inputComponent);
                }
            });
        }
        else
        {
            while (ComponentsManager.Instance.InputQueueCount > 0)
            {
                InputMessage clientMsg = ComponentsManager.Instance.GetFromInputQueue();

                ShapeComponent component = ComponentsManager.Instance.GetComponent<ShapeComponent>(clientMsg.entityId);
                component.speed.x = clientMsg.horizontal * playerSpeed * Time.deltaTime;
                component.speed.y = clientMsg.vertical * playerSpeed * Time.deltaTime;
                ComponentsManager.Instance.SetComponent<ShapeComponent>(clientMsg.entityId, component);

                InputMessage responseMsg = new InputMessage()
                {
                    messageID = 0,
                    timeCreated = Utils.SystemTime,
                    entityId = clientMsg.entityId,
                    senderId = clientMsg.senderId,
                    horizontal = clientMsg.horizontal, // Pas nécessaire. À enlever?
                    vertical = clientMsg.vertical,
                    speed = component.speed, // Envoi de la vitesse attendue suite à l'input
                    pos = component.pos // Envoi de la position attendue suite à l'input
                };
                ComponentsManager.Instance.SetComponent<InputMessage>(clientMsg.entityId, responseMsg);
            }
        }
    }

    public void SendInputMessage(EntityComponent entityID, PlayerComponent playerComponent, ShapeComponent shapeComponent, InputComponent inputComponent)
    {
        if (ECSManager.Instance.NetworkManager.LocalClientId == playerComponent.playerId)
        {
            InputMessage msg = new InputMessage()
            {
                messageID = 0,
                timeCreated = Utils.SystemTime,
                entityId = entityID.id,
                senderId = ECSManager.Instance.NetworkManager.LocalClientId,
                horizontal = inputComponent.horizontal, // Envoi de l'input fait
                vertical = inputComponent.vertical,
                speed = shapeComponent.speed, // Envoi de la vitesse attendue suite à l'input
                pos = shapeComponent.pos // Envoi de la position attendue suite à l'input
            };
            ComponentsManager.Instance.SetComponent<InputMessage>(entityID, msg);

            // Ajout de msg dans l'historique 
            ComponentsManager.Instance.AddToInputHistory(msg);

            if (ComponentsManager.Instance.InputQueueCount > 0)
            {
                InputMessage responseMsg = ComponentsManager.Instance.GetFromInputQueue();
                
                var history = ComponentsManager.Instance.DebugGetInputHistory();
                var queue = ComponentsManager.Instance.DebugGetInputQueue();
                int matchIndex = history.FindIndex(x => x.horizontal == responseMsg.horizontal && x.vertical == responseMsg.vertical);
                history.RemoveRange(0, matchIndex);

                InputMessage correspondingLocalMsg = ComponentsManager.Instance.GetFirstFromInputHistory();

                if ((responseMsg.pos - correspondingLocalMsg.pos).magnitude > PLAYER_RECONCILIATION_THRESHOLD)
                {
                    Debug.Log("The history does not match the server. Must Reconcilitate.");

                }

                ComponentsManager.Instance.RemoveFirstFromInputHistory();

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
