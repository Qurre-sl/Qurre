using Mirror;
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
    }
}