using UnityEngine;

public struct DelayMessage : IComponent
{
    public int messageID;
    public int timeCreated;

    public uint entityId;
    public uint delayMessageId;
    public ulong senderId;
}