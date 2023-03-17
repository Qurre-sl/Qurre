using HarmonyLib;
using MEC;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEngine;

namespace Qurre.Internal.Patches.Round
{
    using PlayerRoles;
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.Start))]
    static class Check
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instr)
        {
            var codes = new List<CodeInstruction>(instr);
            foreach (var code in codes.Select((x, i) => new { Value = x, Index = i }))
            {
                if (code.Value.opcode != OpCodes.Call) continue;
                if (code.Value.operand is not null && code.Value.operand is MethodBase methodBase &&
                    methodBase.Name == nameof(RoundSummary._ProcessServerSideCode))
                    codes[code.Index].operand = AccessTools.Method(typeof(Check), nameof(Check.ProcessServerSide));
            }
            return codes.AsEnumerable();
        }

        static IEnumerator<float> ProcessServerSide(RoundSummary instance)
        {
            float time = Time.unscaledTime;
            while (instance is not null)
            {
                yield return Timing.WaitForSeconds(2.5f);

                while (RoundSummary.RoundLock || !RoundSummary.RoundInProgress() || Time.unscaledTime - time < 15f ||
                    (instance.KeepRoundOnOne && Player.List.Count() < 2) || Round.ElapsedTime.TotalSeconds < 15f)
                    yield return Timing.WaitForSeconds(1);

                yield return Timing.WaitForSeconds(2.5f);

                RoundSummary.SumInfo_ClassList list = default;
                bool end = false;

                foreach (var pl in Player.List)
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
                                if (pl.RoleInfomation.Role is RoleTypeId.Scp0492) list.zombies++;
                                else list.scps_except_zombies++;
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

                int chaos_cf = 0;
                int mtf_cf = 0;
                int scp_cf = 0;

                if (ScpAlive && !MTFAlive && !DClassAlive && !ScientistsAlive && (Qurre.Loader.Configs.RoundEndChaos || !CiAlive))
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
                var winner = LeadingTeam.Draw;

                if (dboys > scientists) chaos_cf++;
                else if (dboys < scientists) mtf_cf++;
                else if (scp > dboys + scientists) scp_cf++;

                if (list.chaos_insurgents > list.mtf_and_guards) chaos_cf++;
                else if (list.chaos_insurgents < list.mtf_and_guards) mtf_cf++;
                else if (scp > list.chaos_insurgents + list.mtf_and_guards) scp_cf++;

                if (chaos_cf > mtf_cf)
                {
                    if (chaos_cf > scp_cf) winner = LeadingTeam.ChaosInsurgency;
                    else if (mtf_cf < scp_cf) winner = LeadingTeam.Anomalies;
                    else winner = LeadingTeam.Draw;
                }
                else if (mtf_cf > chaos_cf)
                {
                    if (mtf_cf > scp_cf) winner = LeadingTeam.FacilityForces;
                    else if (chaos_cf < scp_cf) winner = LeadingTeam.Anomalies;
                    else winner = LeadingTeam.Draw;
                }
                else winner = LeadingTeam.Draw;

                {
                    RoundCheckEvent ev = new(winner, list, end);
                    ev.InvokeEvent();

                    list = ev.Info;
                    instance._roundEnded = ev.End || Round._forceEnd;
                    winner = ev.Winner;
                }

                if (instance._roundEnded)
                {
                    FriendlyFireConfig.PauseDetector = true;

                    string text = $"Round finished! Anomalies: {scp} | Chaos: {list.chaos_insurgents} | " +
                        $"Facility Forces: {list.mtf_and_guards} | D escaped: {dboys} | Scientists escaped: {scientists}";
                    GameCore.Console.AddLog(text, Color.gray);
                    ServerLogs.AddLog(ServerLogs.Modules.Logger, text, ServerLogs.ServerLogType.GameEvent);

                    yield return Timing.WaitForSeconds(0.5f);

                    int wait = Mathf.Clamp(GameCore.ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);
                    if (instance is not null)
                    {
                        var ev = new RoundEndEvent(winner, list, wait);
                        ev.InvokeEvent();

                        winner = ev.Winner;
                        wait = Mathf.Clamp(ev.ToRestart, 5, 1000);

                        instance.RpcShowRoundSummary(instance.classlistStart, list, winner, RoundSummary.EscapedClassD,
                            RoundSummary.EscapedScientists, RoundSummary.KilledBySCPs, wait, (int)GameCore.RoundStart.RoundLength.TotalSeconds);
                    }

                    yield return Timing.WaitForSeconds(wait - 1);

                    instance.RpcDimScreen();
                    Timing.CallDelayed(1f, () => RoundRestarting.RoundRestart.InitiateRoundRestart());

                    // optimization
                    try
                    {
                        foreach (var pl in Player.List)
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
                        foreach (var p in Map.Pickups.ToArray()) try { p.Destroy(); } catch { }
                    }
                    catch { }
                    try
                    {
                        foreach (var doll in Map.Ragdolls.ToArray()) try { doll.Destroy(); } catch { }
                    }
                    catch { }
                    yield break;
                }
            }
        }
    }
}