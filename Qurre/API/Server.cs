using CustomPlayerEffects;
using InventorySystem;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Qurre.API
{
    static public class Server
    {
        static internal Player host;
        static internal Inventory hinv;

        static public ushort Port => ServerStatic.ServerPort;
        static public string Ip => ServerConsole.Ip;

        static public double TPS => Math.Round(1f / Time.smoothDeltaTime);

        static public Player Host
        {
            get
            {
                if (host is null || host.ReferenceHub is null)
                    host = new Player(ReferenceHub.HostHub);

                return host;
            }
        }

        static public Inventory InventoryHost
        {
            get
            {
                if (hinv is null)
                    hinv = ReferenceHub.HostHub.inventory;

                return hinv;
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

        static public float SpawnProtectDuration
        {
            get => SpawnProtected.SpawnDuration;
            set => SpawnProtected.SpawnDuration = value;
        }

        static public List<TObject> GetObjectsOf<TObject>() where TObject : UnityEngine.Object => UnityEngine.Object.FindObjectsOfType<TObject>().ToList();
        static public TObject GetObjectOf<TObject>() where TObject : UnityEngine.Object => UnityEngine.Object.FindObjectOfType<TObject>();

        static public void Restart()
        {
            ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
            RoundRestart.ChangeLevel(true);
        }
        static public void Exit() => Shutdown.Quit();
    }
}