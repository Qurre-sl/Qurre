using System;
using GameCore;
using JetBrains.Annotations;
using RoundRestarting;
using UnityEngine;

namespace Qurre.API.World;

[PublicAPI]
public static class Round
{
    internal static bool ForceEnd;
    internal static bool LocalStarted;
    internal static bool LocalWaiting;

    public static TimeSpan ElapsedTime
        => RoundStart.RoundLength;

    public static DateTime StartedTime
        => DateTime.Now - ElapsedTime;

    public static int CurrentRound { get; internal set; }
    public static int ActiveGenerators { get; internal set; }

    public static short WaitTime
    {
        get => RoundStart.singleton.NetworkTimer;
        set => RoundStart.singleton.NetworkTimer = value;
    }

    public static bool Started
    {
        get
        {
            try
            {
                return LabApi.Features.Wrappers.Round.IsRoundStarted;
            }
            catch
            {
                return LocalStarted;
            }
        }
    }

    public static bool Ended
        => LabApi.Features.Wrappers.Round.IsRoundEnded;

    public static bool Waiting
    {
        get
        {
            try
            {
                if (RoundStart.singleton is null)
                    throw new NullReferenceException("RoundStart.singleton is null");

                return !Started && !Ended;
            }
            catch
            {
                return LocalWaiting;
            }
        }
    }

    public static bool Lock
    {
        get => RoundSummary.RoundLock;
        set => RoundSummary.RoundLock = value;
    }

    public static bool LobbyLock
    {
        get => RoundStart.LobbyLock;
        set => RoundStart.LobbyLock = value;
    }

    public static int EscapedClassD
    {
        get => RoundSummary.EscapedClassD;
        set => RoundSummary.EscapedClassD = value;
    }

    public static int EscapedScientists
    {
        get => RoundSummary.EscapedScientists;
        set => RoundSummary.EscapedScientists = value;
    }

    public static int ScpKills
    {
        get => RoundSummary.KilledBySCPs;
        set => RoundSummary.KilledBySCPs = value;
    }

    public static int RoundKills
    {
        get => RoundSummary.Kills;
        set => RoundSummary.Kills = value;
    }

    public static int ChangedZombies
    {
        get => RoundSummary.ChangedIntoZombies;
        set => RoundSummary.ChangedIntoZombies = value;
    }

    public static void Restart(bool fast = true,
        ServerStatic.NextRoundAction action = ServerStatic.NextRoundAction.DoNothing)
    {
        ServerStatic.StopNextRound = action;
        bool oldFast = CustomNetworkManager.EnableFastRestart;
        CustomNetworkManager.EnableFastRestart = fast;
        RoundRestart.InitiateRoundRestart();
        CustomNetworkManager.EnableFastRestart = oldFast;
    }

    public static void Start()
    {
        CharacterClassManager.ForceRoundStart();
    }

    public static void End()
    {
        ForceEnd = true;
    }

    public static void DimScreen()
    {
        RoundSummary.singleton.RpcDimScreen();
    }

    public static void ShowRoundSummary(RoundSummary.SumInfo_ClassList remainingPlayers, RoundSummary.LeadingTeam team)
    {
        RoundSummary.singleton.RpcShowRoundSummary(RoundSummary.singleton.classlistStart, remainingPlayers, team,
            EscapedClassD, EscapedScientists, ScpKills, seconds: (int)ElapsedTime.TotalSeconds,
            roundCd: Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000));
    }
}