using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using InventorySystem;
using JetBrains.Annotations;
using PlayerStatsSystem;
using Qurre.API.Controllers;
using RoundRestarting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API;

[PublicAPI]
public static class Server
{
    private static Player? _host;
    private static Inventory? _hostInv;

    public static ushort Port
        => ServerStatic.ServerPort;

    public static string Ip
        => ServerConsole.Ip;

    public static double Tps
        => Math.Round(1f / Time.smoothDeltaTime);

    public static Player Host
    {
        get
        {
            if (_host?.ReferenceHub is not null)
                return _host;

            if (!ReferenceHub.TryGetHostHub(out ReferenceHub? hub))
                throw new NullReferenceException("ReferenceHub could not be found");

            _host = new Player(hub);
            return _host;
        }
    }

    public static Inventory InventoryHost
    {
        get
        {
            _hostInv ??= Host.ReferenceHub.inventory;
            return _hostInv;
        }
    }

    public static bool FriendlyFire
    {
        get => ServerConsole.FriendlyFire;
        set
        {
            if (FriendlyFire == value)
                return;

            ServerConsole.FriendlyFire = value;
            ServerConfigSynchronizer.Singleton.RefreshMainBools();
            ServerConfigSynchronizer.OnRefreshed?.Invoke();
            AttackerDamageHandler.RefreshConfigs();

            foreach (Player pl in Player.List)
                pl.FriendlyFire = value;
        }
    }

    public static float SpawnProtectDuration
    {
        get => SpawnProtected.SpawnDuration;
        set => SpawnProtected.SpawnDuration = value;
    }

    public static List<TObject> GetObjectsOf<TObject>() where TObject : Object
    {
        return [.. Object.FindObjectsOfType<TObject>()];
    }

    public static TObject GetObjectOf<TObject>() where TObject : Object
    {
        return Object.FindObjectOfType<TObject>();
    }

    public static void Restart()
    {
        ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
        RoundRestart.ChangeLevel(true);
    }

    public static void Exit()
    {
        Shutdown.Quit();
    }

    internal static void WaitingRefresh()
    {
        _host = null;
        _hostInv = null;
    }
}