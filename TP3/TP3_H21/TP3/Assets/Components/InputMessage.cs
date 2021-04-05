using UnityEngine;

public struct InputMessage : IComponent
{
    public int messageID;
    public int timeCreated;

    public uint entityId;
    public uint inputId;
    public ulong senderId;
    public float horizontal;
    public float vertical;
    public Vector2 speed;
    public Vector2 pos;
}