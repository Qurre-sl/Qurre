using GameCore;
using Respawning;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static RoundSummary;
using static ServerStatic;

namespace Qurre.API
{
    public static class Round
    {
        internal static bool BotSpawned { get; set; } = false;
        internal static bool ForceEnd { get; set; } = false;
        public static TimeSpan ElapsedTime => RoundStart.RoundLength;
        public static DateTime StartedTime => DateTime.Now - ElapsedTime;
        public static int CurrentRound { get; internal set; } = 0;
        public static int ActiveGenerators { get; internal set; } = 0;
        public static float NextRespawn
        {
            get => RespawnManager.Singleton._timeForNextSequence - (float)RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds;
            set => RespawnManager.Singleton._timeForNextSequence = value + (float)RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds;
        }
        public static bool Started { get; } = RoundSummary.RoundInProgress();
        public static bool Ended { get; } = RoundSummary.singleton._roundEnded;
        public static bool Waiting => RoundStart.singleton is not null && !Started && !Ended;
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
        public static int EscapedDPersonnel
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
        public static void Restart(bool IsfastRestart = true, NextRoundAction action = NextRoundAction.DoNothing)
        {
            ServerStatic.StopNextRound = action;
            bool oldfastRestartSetting = CustomNetworkManager.EnableFastRestart;
            CustomNetworkManager.EnableFastRestart = IsfastRestart;
            RoundRestart.InitiateRoundRestart();
            CustomNetworkManager.EnableFastRestart = oldfastRestartSetting;
        }
        public static void Start() => CharacterClassManager.ForceRoundStart();
        public static void End()
        {
            RoundSummary.singleton.ForceEnd();
            ForceEnd = true;
        }
        public static void DimScreen() => RoundSummary.singleton.RpcDimScreen();
        public static void ShowRoundSummary(RoundSummary.SumInfo_ClassList remainingPlayers, LeadingTeam team)
        {
            RoundSummary.singleton.RpcShowRoundSummary
            (
            listStart:RoundSummary.singleton.classlistStart,
            listFinish: remainingPlayers,
            leadingTeam: team,
            eDS: EscapedDPersonnel,
            eSc: EscapedScientists,
            scpKills: ScpKills,
            roundCd: Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000),
            seconds: 10
            );
        } 
        public static void ForceTeamRespawn(bool isCI) => RespawnManager.Singleton.ForceSpawnTeam(isCI ? SpawnableTeamType.ChaosInsurgency : SpawnableTeamType.NineTailedFox);
        public static void CallCICar() => RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.ChaosInsurgency);
        public static void CallMTFHelicopter() => RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.NineTailedFox);
    }
}
