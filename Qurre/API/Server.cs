using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.API
{
    static public class Server
    {
        private static Player host;

        static public ushort Port => ServerStatic.ServerPort;
        static public Player Host
        {
            get
            {
                if (host is null || host.ReferenceHub is null) host = new Player(ReferenceHub.HostHub);
                return host;
            }
        }

        static public bool FriendlyFire
        {
            get => ServerConsole.FriendlyFire;
            set
            {
                if (FriendlyFire == value)
                    return;

                ServerConsole.FriendlyFire = value;
                ServerConfigSynchronizer.Singleton.RefreshMainBools();
                ServerConfigSynchronizer.OnRefreshed?.Invoke();
                PlayerStatsSystem.AttackerDamageHandler.RefreshConfigs();

                foreach (Player pl in Player.List) pl.FriendlyFire = value;
            }
        }

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
    }
}