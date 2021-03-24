using UnityEngine;

public struct InputMessage : IComponent
{
    public int messageID;
    public int timeCreated;

    public uint entityId;
    public float horizontal;
    public float vertical;
}