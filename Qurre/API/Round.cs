using GameCore;
using Respawning;
using RoundRestarting;
using System;
using UnityEngine;

namespace Qurre.API
{
    static public class Round
    {
        static internal bool _forceEnd = false;
        static internal bool _started = false;
        static internal bool _waiting = false;

        static public TimeSpan ElapsedTime => RoundStart.RoundLength;
        static public DateTime StartedTime => DateTime.Now - ElapsedTime;

        static public int CurrentRound { get; internal set; } = 0;
        static public int ActiveGenerators { get; internal set; } = 0;

        static public float NextRespawn
        {
            get => RespawnManager.Singleton._timeForNextSequence - (float)RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds;
            set => RespawnManager.Singleton._timeForNextSequence = value + (float)RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds;
        }

        static public bool Started
        {
            get
            {
                try { return ReferenceHub.LocalHub.characterClassManager.RoundStarted; }
                catch { return _started; }
            }
        }

        static public bool Ended => RoundSummary.singleton._roundEnded;
        static public bool Waiting
        {
            get
            {
                try
                {
                    if (RoundStart.singleton is not null)
                        return !Started && !Ended;
                }
                catch { }

                return _waiting;
            }
        }

        static public bool Lock
        {
            get => RoundSummary.RoundLock;
            set => RoundSummary.RoundLock = value;
        }
        static public bool LobbyLock
        {
            get => RoundStart.LobbyLock;
            set => RoundStart.LobbyLock = value;
        }

        static public int EscapedClassD
        {
            get => RoundSummary.EscapedClassD;
            set => RoundSummary.EscapedClassD = value;
        }
        static public int EscapedScientists
        {
            get => RoundSummary.EscapedScientists;
            set => RoundSummary.EscapedScientists = value;
        }
        static public int ScpKills
        {
            get => RoundSummary.KilledBySCPs;
            set => RoundSummary.KilledBySCPs = value;
        }
        static public int RoundKills
        {
            get => RoundSummary.Kills;
            set => RoundSummary.Kills = value;
        }
        static public int ChangedZombies
        {
            get => RoundSummary.ChangedIntoZombies;
            set => RoundSummary.ChangedIntoZombies = value;
        }

        static public void Restart(bool fast = true, ServerStatic.NextRoundAction action = ServerStatic.NextRoundAction.DoNothing)
        {
            ServerStatic.StopNextRound = action;
            bool oldfast = CustomNetworkManager.EnableFastRestart;
            CustomNetworkManager.EnableFastRestart = fast;
            RoundRestart.InitiateRoundRestart();
            CustomNetworkManager.EnableFastRestart = oldfast;
        }
        static public void Start()
            => CharacterClassManager.ForceRoundStart();
        static public void End()
            => _forceEnd = true;

        static public void DimScreen() => RoundSummary.singleton.RpcDimScreen();
        static public void ShowRoundSummary(RoundSummary.SumInfo_ClassList remainingPlayers, LeadingTeam team) =>
            RoundSummary.singleton.RpcShowRoundSummary(RoundSummary.singleton.classlistStart, remainingPlayers, team,
                EscapedClassD, EscapedScientists, ScpKills, seconds: (int)ElapsedTime.TotalSeconds,
                roundCd: Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000));

        static public void ForceTeamRespawn(bool isCI) => RespawnManager.Singleton.ForceSpawnTeam(isCI ? SpawnableTeamType.ChaosInsurgency : SpawnableTeamType.NineTailedFox);
        static public void CallCICar() => RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.ChaosInsurgency);
        static public void CallMTFHelicopter() => RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.NineTailedFox);
    }
}