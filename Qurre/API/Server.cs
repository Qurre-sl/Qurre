using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using PlayerStatsSystem;
using RoundRestarting;
using UnityEngine;

namespace Qurre.API
{
    public static class Server
    {
        internal static Player host;
        internal static Inventory hinv;

        public static ushort Port => ServerStatic.ServerPort;
        public static string Ip => ServerConsole.Ip;

        public static Player Host
        {
            get
            {
                if (host is null || host.ReferenceHub is null)
                {
                    host = new (ReferenceHub.HostHub);
                }

                return host;
            }
        }

        public static Inventory InventoryHost
        {
            get
            {
                if (hinv is null)
                {
                    hinv = ReferenceHub.HostHub.inventory;
                }

                return hinv;
            }
        }

        public static bool FriendlyFire
        {
            get => ServerConsole.FriendlyFire;
            set
            {
                if (FriendlyFire == value)
                {
                    return;
                }

                ServerConsole.FriendlyFire = value;
                ServerConfigSynchronizer.Singleton.RefreshMainBools();
                ServerConfigSynchronizer.OnRefreshed?.Invoke();
                AttackerDamageHandler.RefreshConfigs();

                foreach (Player pl in Player.List)
                {
                    pl.FriendlyFire = value;
                }
            }
        }

        public static List<TObject> GetObjectsOf<TObject>() where TObject : Object => Object.FindObjectsOfType<TObject>().ToList();

        public static TObject GetObjectOf<TObject>() where TObject : Object => Object.FindObjectOfType<TObject>();

        public static void Restart()
        {
            ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
            RoundRestart.ChangeLevel(true);
        }

        public static void Exit() => Shutdown.Quit();
    }
}