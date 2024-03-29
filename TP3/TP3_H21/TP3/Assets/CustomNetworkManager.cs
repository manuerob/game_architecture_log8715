﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;

public class CustomNetworkManager : NetworkingManager
{
    public void Awake()
    {
        OnClientConnectedCallback += OnClientConnected;
        OnServerStarted += OnStartServer;
    }
    
    public void OnClientConnected(ulong clientId)
    {
        if (isServer)
        {
            bool spawnFound = ComponentsManager.Instance.TryGetComponent(new EntityComponent(0), out SpawnInfo spawnInfo);

            if (!spawnFound)
            {
                spawnInfo = new SpawnInfo(false);
            }
            spawnInfo.playersToSpawn.Add((uint)clientId);
            ComponentsManager.Instance.SetComponent<SpawnInfo>(new EntityComponent(0), spawnInfo);
        }
        else
        {
            RegisterClientNetworkHandlers();
        }
    }

    public void OnStartServer()
    {
        RegisterServerNetworkHandlers();
    }

    public void SendReplicationMessage(ReplicationMessage msg)
    {
        using (PooledBitStream stream = PooledBitStream.Get())
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteInt32(msg.messageID);
                writer.WriteInt32(msg.timeCreated);
                writer.WriteUInt32(msg.entityId);
                writer.WriteInt16((byte)msg.shape);
                writer.WriteVector2(msg.pos);
                writer.WriteVector2(msg.speed);
                writer.WriteDouble(msg.size);
                CustomMessagingManager.SendNamedMessage("Replication", null, stream, "customChannel");
            }
        }
    }

    public void SendInputMessage(InputMessage msg, bool sendToServer)
    {
        using (PooledBitStream stream = PooledBitStream.Get())
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteInt32(msg.messageID);
                writer.WriteInt32(msg.timeCreated);
                writer.WriteUInt32(msg.entityId);
                writer.WriteUInt32(msg.inputId);
                writer.WriteUInt64(msg.senderId);
                writer.WriteDouble(msg.horizontal);
                writer.WriteDouble(msg.vertical);
                writer.WriteVector2(msg.pos);
                writer.WriteVector2(msg.speed);

                if (sendToServer)
                {
                    Debug.Log("Sending input from " + LocalClientId.ToString() + " to server " + ServerClientId.ToString());
                    CustomMessagingManager.SendNamedMessage("Input", ServerClientId, stream, "customChannel");
                }
                else
                {
                    Debug.Log("Sending input from server to client " + msg.entityId.ToString());
                    CustomMessagingManager.SendNamedMessage("Input", msg.senderId, stream, "customChannel");
                }
            }
        }
    }

    public void SendDelayMessage(DelayMessage msg, bool sendToServer)
    {
        using (PooledBitStream stream = PooledBitStream.Get())
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteInt32(msg.messageID);
                writer.WriteInt32(msg.timeCreated);
                writer.WriteUInt32(msg.entityId);
                writer.WriteUInt32(msg.delayMessageId);
                writer.WriteUInt64(msg.senderId);

                if (sendToServer)
                {
                    Debug.Log("Sending input from " + LocalClientId.ToString() + " to server " + ServerClientId.ToString());
                    CustomMessagingManager.SendNamedMessage("Delay", ServerClientId, stream, "customChannel");
                }
                else
                {
                    Debug.Log("Sending input from server to client " + msg.entityId.ToString());
                    CustomMessagingManager.SendNamedMessage("Delay", msg.senderId, stream, "customChannel");
                }
            }
        }
    }

    private void HandleReplicationMessage(ulong clientId, Stream stream)
    {
        ReplicationMessage replicationMessage = new ReplicationMessage();
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            replicationMessage.messageID = reader.ReadInt32();
            replicationMessage.timeCreated = reader.ReadInt32();
            replicationMessage.entityId = reader.ReadUInt32();
            replicationMessage.shape = (Config.Shape)reader.ReadInt16();
            replicationMessage.pos = reader.ReadVector2();
            replicationMessage.speed = reader.ReadVector2();
            replicationMessage.size = (float)reader.ReadDouble();
            ComponentsManager.Instance.SetComponent<ReplicationMessage>(replicationMessage.entityId, replicationMessage);
            if (!ComponentsManager.Instance.EntityContains<EntityComponent>(replicationMessage.entityId))
            {
                bool spawnFound = ComponentsManager.Instance.TryGetComponent(new EntityComponent(0), out SpawnInfo spawnInfo);

                if (!spawnFound)
                {
                    spawnInfo = new SpawnInfo(false);
                }
                spawnInfo.replicatedEntitiesToSpawn.Add(replicationMessage);
                ComponentsManager.Instance.SetComponent<SpawnInfo>(new EntityComponent(0), spawnInfo);
            }
        }
    }

    public void RegisterClientNetworkHandlers()
    {
        CustomMessagingManager.RegisterNamedMessageHandler("Replication", HandleReplicationMessage);
        CustomMessagingManager.RegisterNamedMessageHandler("Input", HandleInputMessage);
        CustomMessagingManager.RegisterNamedMessageHandler("Delay", HandleDelayMessage);
    }

    private void HandleInputMessage(ulong clientId, Stream stream)
    {
        InputMessage inputMessage = new InputMessage();
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            inputMessage.messageID = reader.ReadInt32();
            inputMessage.timeCreated = reader.ReadInt32();
            inputMessage.entityId = reader.ReadUInt32();
            inputMessage.inputId = reader.ReadUInt32();
            inputMessage.senderId = reader.ReadUInt64();
            inputMessage.horizontal = (float)reader.ReadDouble();
            inputMessage.vertical = (float)reader.ReadDouble();
            inputMessage.pos = reader.ReadVector2();
            inputMessage.speed = reader.ReadVector2();

            ComponentsManager.Instance.AddToInputQueue(inputMessage);
            Debug.Log("Adding input to queue from client " + inputMessage.senderId.ToString());
        }
    }

    private void HandleDelayMessage(ulong clientId, Stream stream)
    {
        DelayMessage delayMessage = new DelayMessage();
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            delayMessage.messageID = reader.ReadInt32();
            delayMessage.timeCreated = reader.ReadInt32();
            delayMessage.entityId = reader.ReadUInt32();
            delayMessage.delayMessageId = reader.ReadUInt32();
            delayMessage.senderId = reader.ReadUInt64();

            ComponentsManager.Instance.AddToDelayQueue(delayMessage);
        }
    }

    public void RegisterServerNetworkHandlers()
    {
        CustomMessagingManager.RegisterNamedMessageHandler("Input", HandleInputMessage);
        CustomMessagingManager.RegisterNamedMessageHandler("Delay", HandleDelayMessage);
    }

    public new bool isServer { get { return GetConnectionStatus() == ConnectionStatus.isServer; } }
    public new bool isClient { get { return GetConnectionStatus() == ConnectionStatus.isClient; } }

    public enum ConnectionStatus
    {
        isClient,
        isServer,
        notConnected
    }

    public ConnectionStatus GetConnectionStatus()
    {
        if (IsConnectedClient)
        {
            return ConnectionStatus.isClient;
        }
        else if (IsServer && IsListening)
        {
            return ConnectionStatus.isServer;
        }
        else
        {
            return ConnectionStatus.notConnected;
        }
    }
}
