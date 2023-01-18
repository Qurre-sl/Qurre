using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GameCore;
using HarmonyLib;
using MEC;
using PlayerRoles;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using RoundRestarting;
using UnityEngine;

namespace Qurre.Internal.Patches.Round
{
    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.Start))]
    internal static class Check
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instr)
        {
            var codes = new List<CodeInstruction>(instr);

            foreach (var code in codes.Select((x, i) => new { Value = x, Index = i }))
            {
                if (code.Value.opcode != OpCodes.Call)
                {
                    continue;
                }

                if (code.Value.operand is not null && code.Value.operand is MethodBase methodBase && methodBase.Name == nameof(RoundSummary._ProcessServerSideCode))
                {
                    codes[code.Index].operand = AccessTools.Method(typeof(Check), nameof(ProcessServerSide));
                }
            }

            return codes.AsEnumerable();
        }

        private static IEnumerator<float> ProcessServerSide(RoundSummary instance)
        {
            float time = Time.unscaledTime;

            while (instance is not null)
            {
                yield return Timing.WaitForSeconds(2.5f);
                while (RoundSummary.RoundLock || !RoundSummary.RoundInProgress() || Time.unscaledTime - time < 15f || instance.KeepRoundOnOne && API.Player.List.Count() < 2)
                    yield return Timing.WaitForSeconds(1);

                RoundSummary.SumInfo_ClassList list = default;
                var end = false;

                foreach (API.Player pl in API.Player.List)
                {
                    switch (pl.RoleInfomation.Team)
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
                            if (pl.RoleInfomation.Role is RoleTypeId.Scp0492)
                            {
                                list.zombies++;
                            }
                            else
                            {
                                list.scps_except_zombies++;
                            }

                            break;
                        }
                    }
                }

                yield return float.NegativeInfinity;
                list.warhead_kills = AlphaWarheadController.Detonated ? AlphaWarheadController.Singleton.WarheadKills : -1;
                yield return float.NegativeInfinity;

                int scp = list.scps_except_zombies + list.zombies;
                int dboys = RoundSummary.EscapedClassD + list.class_ds;
                int scientists = RoundSummary.EscapedScientists + list.scientists;

                bool MTFAlive = list.mtf_and_guards > 0;
                bool CiAlive = list.chaos_insurgents > 0;
                bool ScpAlive = scp > 0;
                bool DClassAlive = list.class_ds > 0;
                bool ScientistsAlive = list.scientists > 0;

                var chaos_cf = 0;
                var mtf_cf = 0;
                var scp_cf = 0;

                if (ScpAlive && !MTFAlive && !DClassAlive && !ScientistsAlive)
                {
                    end = true;
                    scp_cf++;
                }
                else if (!ScpAlive && (MTFAlive || ScientistsAlive) && !DClassAlive && !CiAlive)
                {
                    end = true;
                    mtf_cf++;
                }
                else if (!ScpAlive && !MTFAlive && !ScientistsAlive && (DClassAlive || CiAlive))
                {
                    end = true;
                    chaos_cf++;
                }
                else if (!ScpAlive && !MTFAlive && !ScientistsAlive && !DClassAlive && !CiAlive)
                {
                    end = true;
                }

                // real winner of the round
                LeadingTeam winner = LeadingTeam.Draw;

                if (dboys > scientists)
                {
                    chaos_cf++;
                }
                else if (dboys < scientists)
                {
                    mtf_cf++;
                }
                else if (scp > dboys + scientists)
                {
                    scp_cf++;
                }

                if (list.chaos_insurgents > list.mtf_and_guards)
                {
                    chaos_cf++;
                }
                else if (list.chaos_insurgents < list.mtf_and_guards)
                {
                    mtf_cf++;
                }
                else if (scp > list.chaos_insurgents + list.mtf_and_guards)
                {
                    scp_cf++;
                }

                if (chaos_cf > mtf_cf)
                {
                    if (chaos_cf > scp_cf)
                    {
                        winner = LeadingTeam.ChaosInsurgency;
                    }
                    else if (mtf_cf < scp_cf)
                    {
                        winner = LeadingTeam.Anomalies;
                    }
                    else
                    {
                        winner = LeadingTeam.Draw;
                    }
                }
                else if (mtf_cf > chaos_cf)
                {
                    if (mtf_cf > scp_cf)
                    {
                        winner = LeadingTeam.FacilityForces;
                    }
                    else if (chaos_cf < scp_cf)
                    {
                        winner = LeadingTeam.Anomalies;
                    }
                    else
                    {
                        winner = LeadingTeam.Draw;
                    }
                }
                else
                {
                    winner = LeadingTeam.Draw;
                }

                {
                    RoundCheckEvent ev = new (winner, list, end);
                    ev.InvokeEvent();

                    list = ev.Info;
                    instance._roundEnded = ev.End || API.Round._forceEnd;
                    winner = ev.Winner;
                }

                if (instance._roundEnded)
                {
                    FriendlyFireConfig.PauseDetector = true;

                    string text = $"Round finished! Anomalies: {scp} | Chaos: {list.chaos_insurgents} | "
                                  + $"Facility Forces: {list.mtf_and_guards} | D escaped: {dboys} | Scientists escaped: {scientists}";
                    Console.AddLog(text, Color.gray);
                    ServerLogs.AddLog(ServerLogs.Modules.Logger, text, ServerLogs.ServerLogType.GameEvent);

                    yield return Timing.WaitForSeconds(0.5f);

                    int wait = Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);

                    if (instance is not null)
                    {
                        var ev = new RoundEndEvent(winner, list, wait);
                        ev.InvokeEvent();

                        winner = ev.Winner;
                        wait = Mathf.Clamp(ev.ToRestart, 5, 1000);

                        instance.RpcShowRoundSummary(
                            instance.classlistStart, list, winner, RoundSummary.EscapedClassD,
                            RoundSummary.EscapedScientists, RoundSummary.KilledBySCPs, wait, (int)RoundStart.RoundLength.TotalSeconds);
                    }

                    yield return Timing.WaitForSeconds(wait - 1);

                    instance.RpcDimScreen();
                    Timing.CallDelayed(1f, () => RoundRestart.InitiateRoundRestart());

                    // optimization
                    try
                    {
                        foreach (API.Player pl in API.Player.List)
                        {
                            try
                            {
                                if (pl.RoleInfomation.Role != RoleTypeId.Spectator)
                                {
                                    pl.Inventory.Clear();
                                    pl.RoleInfomation.Role = RoleTypeId.Spectator;
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }

                    try
                    {
                        foreach (Pickup p in API.Map.Pickups.ToArray())
                        {
                            try
                            {
                                p.Destroy();
                            }
                            catch { }
                        }
                    }
                    catch { }

                    try
                    {
                        foreach (Ragdoll doll in API.Map.Ragdolls.ToArray())
                        {
                            try
                            {
                                doll.Destroy();
                            }
                            catch { }
                        }
                    }
                    catch { }

                    yield break;
                }
            }
        }
    }
}