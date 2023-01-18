using System;
using System.Reflection;
using Mirror;

namespace Qurre.API
{
    public static class Network // rename later
    {
        private static MethodInfo sendSpawnMessage;

        public static MethodInfo SendSpawnMessage
        {
            get
            {
                if (sendSpawnMessage is null)
                {
                    sendSpawnMessage = typeof(NetworkServer).GetMethod(
                        "SendSpawnMessage", BindingFlags.Instance
                                            | BindingFlags.InvokeMethod
                                            | BindingFlags.NonPublic
                                            | BindingFlags.Static
                                            | BindingFlags.Public);
                }

                return sendSpawnMessage;
            }
        }

        public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public;
            MethodInfo info = type.GetMethod(methodName, flags);
            info?.Invoke(null, param);
        }


        public static void UpdateData(this NetworkIdentity identity) => NetworkServer.SendToAll(identity.SpawnMessage());

        public static SpawnMessage SpawnMessage(this NetworkIdentity identity)
        {
            PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
            PooledNetworkWriter writer2 = NetworkWriterPool.GetWriter();
            ArraySegment<byte> payload = NetworkServer.CreateSpawnMessagePayload(false, identity, writer, writer2);

            return new()
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