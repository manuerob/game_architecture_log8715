using UnityEngine;

public struct InputComponent : IComponent
{
    public uint entityId;
    public float horizontal;
    public float vertical;
}