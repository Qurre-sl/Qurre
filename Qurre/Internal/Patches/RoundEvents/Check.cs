using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GameCore;
using HarmonyLib;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using Qurre.Loader;
using RoundRestarting;
using UnityEngine;
using Console = GameCore.Console;
using Log = Qurre.API.Log;
using Map = LabApi.Features.Wrappers.Map;
using Player = Qurre.API.Controllers.Player;
using Round = Qurre.API.World.Round;
using Server = Qurre.API.Server;

namespace Qurre.Internal.Patches.RoundEvents;

[HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.Start))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Check
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instr)
    {
        var codes = new List<CodeInstruction>(instr);
        foreach (var code in codes.Select((x, i) => new { Value = x, Index = i }))
        {
            if (code.Value.opcode != OpCodes.Call) continue;
            if (code.Value.operand is MethodBase { Name: nameof(RoundSummary._ProcessServerSideCode) })
                codes[code.Index].operand = AccessTools.Method(typeof(Check), nameof(ProcessServerSide));
        }

        return codes.AsEnumerable();
    }

    private static IEnumerator<float> ProcessServerSide(RoundSummary instance)
    {
        float time = Time.unscaledTime;

        if (instance != null)
            instance.IsRoundEnded = false;

        while (instance != null)
        {
            yield return Timing.WaitForSeconds(2.5f);

            while (RoundSummary.RoundLock || !Round.Started || Time.unscaledTime - time < 15f ||
                   (instance.KeepRoundOnOne && Player.List.Count() < 2) || Round.ElapsedTime.TotalSeconds < 15f)
                yield return Timing.WaitForSeconds(1);

            RoundSummary.SumInfo_ClassList list = default;
            bool end = false;

            foreach (Player? pl in Player.List)
                try
                {
                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (pl.RoleInformation.Team)
                    {
                        case Team.ClassD:
                            list.class_ds++;
                            break;
                        case Team.Scientists:
                            list.scientists++;
                            break;
                        case Team.ChaosInsurgency:
                            list.chaos_insurgents++;
                            break;
                        case Team.FoundationForces:
                            list.mtf_and_guards++;
                            break;
                        case Team.SCPs:
                            {
                                if (pl.RoleInformation.Role is RoleTypeId.Scp0492)
                                    list.zombies++;
                                else
                                    list.scps_except_zombies++;
                                break;
                            }
                    }
                }
                catch
                {
                    // ignored
                }

            yield return float.NegativeInfinity;
            list.warhead_kills = AlphaWarheadController.Detonated ? AlphaWarheadController.Singleton.WarheadKills : -1;
            yield return float.NegativeInfinity;

            int scp = list.scps_except_zombies + list.zombies;
            int dBoys = RoundSummary.EscapedClassD + list.class_ds;
            int scientists = RoundSummary.EscapedScientists + list.scientists;

            bool mtfAlive = list.mtf_and_guards > 0;
            bool chaosAlive = list.chaos_insurgents > 0;
            bool scpAlive = scp > 0;
            bool dClassAlive = list.class_ds > 0;
            bool scientistsAlive = list.scientists > 0;

            int chaosCf = 0;
            int mtfCf = 0;
            int scpCf = 0;

            switch (scpAlive)
            {
                case true when !mtfAlive && !dClassAlive && !scientistsAlive && (Configs.RoundEndChaos || !chaosAlive):
                    end = true;
                    scpCf++;
                    break;
                case false when (mtfAlive || scientistsAlive) && !dClassAlive && !chaosAlive:
                    end = true;
                    mtfCf++;
                    break;
                case false when !mtfAlive && !scientistsAlive && (dClassAlive || chaosAlive):
                    end = true;
                    chaosCf++;
                    break;
                case false when !mtfAlive && !scientistsAlive && !dClassAlive && !chaosAlive:
                    end = true;
                    break;
            }

            // real winner of the round
            RoundSummary.LeadingTeam winner;

            if (dBoys > scientists) chaosCf++;
            else if (dBoys < scientists) mtfCf++;
            else if (scp > dBoys + scientists) scpCf++;

            if (list.chaos_insurgents > list.mtf_and_guards) chaosCf++;
            else if (list.chaos_insurgents < list.mtf_and_guards) mtfCf++;
            else if (scp > list.chaos_insurgents + list.mtf_and_guards) scpCf++;

            if (chaosCf > mtfCf)
            {
                if (chaosCf > scpCf) winner = RoundSummary.LeadingTeam.ChaosInsurgency;
                else if (mtfCf < scpCf) winner = RoundSummary.LeadingTeam.Anomalies;
                else winner = RoundSummary.LeadingTeam.Draw;
            }
            else if (mtfCf > chaosCf)
            {
                if (mtfCf > scpCf) winner = RoundSummary.LeadingTeam.FacilityForces;
                else if (chaosCf < scpCf) winner = RoundSummary.LeadingTeam.Anomalies;
                else winner = RoundSummary.LeadingTeam.Draw;
            }
            else
            {
                winner = RoundSummary.LeadingTeam.Draw;
            }

            RoundCheckEvent evCheck = new(winner, list, end);
            evCheck.InvokeEvent();

            list = evCheck.Info;
            instance.IsRoundEnded = evCheck.End || Round.ForceEnd;
            winner = evCheck.Winner;

            if (!instance.IsRoundEnded)
                continue;

            FriendlyFireConfig.PauseDetector = true;

            string text = $"Round finished! Anomalies: {scp} | Chaos: {list.chaos_insurgents} | " +
                          $"Facility Forces: {list.mtf_and_guards} | D escaped: {dBoys} | Scientists escaped: {scientists}";
            Console.AddLog(text, Color.gray);
            ServerLogs.AddLog(ServerLogs.Modules.GameLogic, text, ServerLogs.ServerLogType.GameEvent);

            yield return Timing.WaitForSeconds(0.5f);

            int wait = Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);

            RoundEndEvent evEnd = new(winner, list, wait);
            evEnd.InvokeEvent();

            winner = evEnd.Winner;
            wait = Mathf.Clamp(evEnd.ToRestart, 5, 1000);

            instance.RpcShowRoundSummary(instance.classlistStart, list, winner, RoundSummary.EscapedClassD,
                RoundSummary.EscapedScientists, RoundSummary.KilledBySCPs, wait,
                (int)RoundStart.RoundLength.TotalSeconds);

            yield return Timing.WaitForSeconds(wait - 1);

            instance.RpcDimScreen();
            Timing.CallDelayed(1f, () =>
            {
                try
                {
                    RoundRestart.InitiateRoundRestart();
                }
                catch (Exception ex)
                {
                    Log.Error("Round restart error in game method [InitiateRoundRestart]:\n" + ex);
                    Server.Restart();
                }
            });

            // optimization
            try
            {
                foreach (Player? pl in Player.List)
                    try
                    {
                        if (pl.RoleInformation.Role == RoleTypeId.Spectator)
                            continue;

                        pl.Inventory.Clear();
                        pl.RoleInformation.Role = RoleTypeId.Spectator;
                    }
                    catch
                    {
                        // ignored
                    }
            }
            catch
            {
                // ignored
            }

            try
            {
                foreach (Pickup? p in Map.Pickups.ToArray())
                    try
                    {
                        p.Destroy();
                    }
                    catch
                    {
                        // ignored
                    }
            }
            catch
            {
                // ignored
            }

            try
            {
                foreach (Corpse? doll in API.World.Map.Corpses.ToArray())
                    try
                    {
                        doll.Destroy();
                    }
                    catch
                    {
                        // ignored
                    }
            }
            catch
            {
                // ignored
            }

            try
            {
                foreach (Primitive? prim in API.World.Map.Primitives.ToArray())
                    try
                    {
                        prim.Destroy();
                    }
                    catch
                    {
                        // ignored
                    }
            }
            catch
            {
                // ignored
            }

            yield break;
        } // end while
    } // end void
}