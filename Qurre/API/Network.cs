using Mirror;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Qurre.API
{
    static public class Network // rename later
    {
        static MethodInfo sendSpawnMessage;
        static public MethodInfo SendSpawnMessage
        {
            get
            {
                if (sendSpawnMessage is null)
                    sendSpawnMessage = typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.Instance | BindingFlags.InvokeMethod
                        | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
                return sendSpawnMessage;
            }
        }

        static public void InvokeStaticMethod(this Type type, string methodName, object[] param)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Public;
            MethodInfo info = type.GetMethod(methodName, flags);
            info?.Invoke(null, param);
        }


        static public void UpdateDataForConnection(this NetworkIdentity identity, NetworkConnectionToClient connection)
        {
            if (!connection.isReady)
                return;

            var message = identity.SpawnMessage();

            using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
            NetworkMessages.Pack(message, networkWriterPooled);
            ArraySegment<byte> segment = networkWriterPooled.ToArraySegment();

            connection.Send(segment);
        }

        static public void UpdateData(this NetworkIdentity identity)
            => NetworkServer.SendToAll(identity.SpawnMessage());

        static public SpawnMessage SpawnMessage(this NetworkIdentity identity)
        {
            var writer = NetworkWriterPool.Get();
            var writer2 = NetworkWriterPool.Get();
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
}