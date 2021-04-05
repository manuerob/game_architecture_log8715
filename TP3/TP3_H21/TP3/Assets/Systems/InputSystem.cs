using UnityEngine;

// Il faudrait que ce système prenne les inputs, update la vitesse, ajoute un composant d'inputs (vertical et horizontal)
// Ensuite, après un système qui met à jour la position, on aurait un autre système qui récupère le composant d'input pour créer le message
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
                    // shapeComponent.pos.x += horizontal * speed * Time.deltaTime;
                    // shapeComponent.pos.y += vertical * speed * Time.deltaTime;
                    shapeComponent.speed.x = horizontal * speed * Time.deltaTime;
                    shapeComponent.speed.y = vertical * speed * Time.deltaTime;
                    ComponentsManager.Instance.SetComponent<ShapeComponent>(entityID, shapeComponent);

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
                /*
                InputMessage msg = new InputMessage()
                {
                    messageID = 0,
                    timeCreated = Utils.SystemTime,
                    entityId = entityID.id,
                    horizontal = horizontal, // Envoi de l'input fait
                    vertical = vertical,
                    speed = shapeComponent.speed, // Envoi de la vitesse attendue suite à l'input
                    pos = shapeComponent.pos // Envoi de la position attendue suite à l'input (Pas encore calculé, il faut updater le message...)
                };
                ComponentsManager.Instance.SetComponent<InputMessage>(entityID, msg);

                // Ajout de msg dans l'historique 
                ComponentsManager.Instance.AddToInputHistory(msg);

                if (ComponentsManager.Instance.InputQueueCount > 0)
                {
                    InputMessage responseMsg = ComponentsManager.Instance.GetFromInputQueue();
                    InputMessage correspondingLocalMsg = ComponentsManager.Instance.GetFirstFromInputHistory();

                    if ((responseMsg.pos - correspondingLocalMsg.pos).magnitude > (float.Epsilon * 10))
                    {
                        Debug.LogWarning("The history does not match the server. Must Reconcilitate.");
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

        });
            }*/
                else
                {
                    while (ComponentsManager.Instance.InputQueueCount > 0)
                    {
                        InputMessage clientMsg = ComponentsManager.Instance.GetFromInputQueue();

                        ShapeComponent component = ComponentsManager.Instance.GetComponent<ShapeComponent>(clientMsg.entityId);
                        component.speed.x = clientMsg.horizontal * speed * Time.deltaTime;
                        component.speed.y = clientMsg.vertical * speed * Time.deltaTime;
                        ComponentsManager.Instance.SetComponent<ShapeComponent>(clientMsg.entityId, component);

                        InputMessage responseMsg = new InputMessage()
                        {
                            messageID = 0,
                            timeCreated = Utils.SystemTime,
                            entityId = clientMsg.entityId,
                            horizontal = clientMsg.horizontal, // Pas nécessaire. À enlever?
                            vertical = clientMsg.vertical,
                            speed = component.speed, // Envoi de la vitesse attendue suite à l'input
                            pos = component.pos // Envoi de la position attendue suite à l'input
                        };
                        ComponentsManager.Instance.SetComponent<InputMessage>(clientMsg.entityId, responseMsg);
                    }
                }
            });
        }
    }
}