using System;
using System.Reflection;
using JetBrains.Annotations;
using Mirror;

namespace Qurre.API;

[PublicAPI]
public static class Network
{
    private static MethodInfo? _sendSpawnMessage;

    public static MethodInfo? SendSpawnMessage
    {
        get
        {
            _sendSpawnMessage ??= typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.Instance |
                BindingFlags.InvokeMethod |
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
            return _sendSpawnMessage;
        }
    }

    public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic |
                                   BindingFlags.Static | BindingFlags.Public;
        MethodInfo? info = type.GetMethod(methodName, flags);
        info?.Invoke(null, param);
    }


    public static void SendDataToClient<T>(this NetworkConnectionToClient connection, T message)
        where T : struct, NetworkMessage
    {
        if (!connection.isReady)
            return;

        using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
        NetworkMessages.Pack(message, networkWriterPooled);
        var segment = networkWriterPooled.ToArraySegment();

        connection.Send(segment);
    }

    public static void UpdateDataForConnection(this NetworkIdentity identity, NetworkConnectionToClient connection)
    {
        if (!connection.isReady)
            return;

        SpawnMessage message = identity.SpawnMessage();

        connection.SendDataToClient(message);
    }

    public static void UpdateData(this NetworkIdentity identity)
    {
        NetworkServer.SendToAll(identity.SpawnMessage());
    }

    public static SpawnMessage SpawnMessage(this NetworkIdentity identity)
    {
        NetworkWriterPooled? writer = NetworkWriterPool.Get();
        NetworkWriterPooled? writer2 = NetworkWriterPool.Get();
        var payload = NetworkServer.CreateSpawnMessagePayload(false, identity, writer, writer2);

        return new SpawnMessage
        {
            netId = identity.netId,
            isLocalPlayer = false,
            isOwner = false,
            sceneId = identity.sceneId,
            assetId = identity.assetId,
            position = identity.gameObject.transform.position,
            rotation = identity.gameObject.transform.rotation,
            scale = identity.gameObject.transform.localScale,
            payload = payload
        };
    }
}