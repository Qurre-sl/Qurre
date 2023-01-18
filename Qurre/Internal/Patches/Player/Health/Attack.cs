using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Health
{
    [HarmonyPatch(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.ProcessDamage))]
    internal static class Attack
    {
        internal static void HandlerDamage(AttackerDamageHandler handler, ReferenceHub target, bool allowed)
        {
            try
            {
                API.Player attacker = handler.Attacker.Hub.GetPlayer();

                if (attacker.FriendlyFire)
                {
                    handler.IsFriendlyFire = false;
                }

                AttackEvent ev = new (attacker, target.GetPlayer(), handler, handler.Damage, handler.IsFriendlyFire, allowed);
                ev.InvokeEvent();

                if (ev.Damage == -1)
                {
                    ev.Damage = ev.Target.HealthInfomation.Hp + 1;
                }

                handler.Damage = ev.Damage;
                handler.IsFriendlyFire = ev.FriendlyFire;

                if (!ev.Allowed)
                {
                    handler.Damage = 0;
                }

                if (ev.FriendlyFire)
                {
                    if (!API.Server.FriendlyFire)
                    {
                        handler.Damage = 0;
                    }
                    else
                    {
                        handler.Damage *= AttackerDamageHandler._ffMultiplier;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Health}} [Attack]:{e}\n{e.StackTrace}");
            }
        }

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            Label metLabel = retLabel;
            {
                for (var iq = 0; iq < instructions.Count(); iq++)
                {
                    CodeInstruction ins = instructions.ElementAt(iq);

                    if (ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase && methodBase.Name == nameof(StandardDamageHandler.ProcessDamage))
                    {
                        metLabel = instructions.ElementAt(iq - 2).labels[0];
                    }
                }
            }

            LocalBuilder allow = generator.DeclareLocal(typeof(bool));

            List<CodeInstruction> list = new ();

            var level = 0;

            for (var i = 0; i < instructions.Count(); i++)
            {
                CodeInstruction ins = instructions.ElementAt(i);

                // set "allowed = false" instant "ev.Damage = 0"
                if (level == 0 || level == 1)
                {
                    if (ins.opcode == OpCodes.Callvirt
                        && ins.operand is not null
                        && ins.operand is MethodBase methodBase
                        && methodBase == AccessTools.PropertySetter(typeof(StandardDamageHandler), nameof(StandardDamageHandler.Damage)))
                    {
                        level++;

                        list.RemoveRange(i - 2, list.Count - (i - 2));
                        list.AddRange(
                            new[] // bool allowed = false; goto: method;
                            {
                                new (OpCodes.Nop), // balance list count: 4 -> 4
                                new CodeInstruction(OpCodes.Ldc_I4_0),
                                new CodeInstruction(OpCodes.Stloc, allow.LocalIndex),
                                new CodeInstruction(OpCodes.Br, metLabel)
                            });
                        i++;
                        continue;
                    }
                }
                // remove "ev.Damage *= FFMultiplier"
                else if (level == 2)
                {
                    if (ins.opcode == OpCodes.Callvirt
                        && ins.operand is not null
                        && ins.operand is MethodBase methodBase
                        && methodBase == AccessTools.PropertySetter(typeof(StandardDamageHandler), nameof(StandardDamageHandler.Damage)))
                    {
                        level++;
                        list.RemoveRange(i - 5, list.Count - (i - 5));
                        continue;
                    }
                }

                list.Add(ins);
            }

            list.InsertRange(
                0, new[] // bool allowed = true;
                {
                    new (OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Stloc, allow.LocalIndex)
                });

            int index = list.FindLastIndex(
                ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase && methodBase.Name == nameof(StandardDamageHandler.ProcessDamage));
            list.InsertRange(
                index == -1 ? list.Count - 3 : index - 2, new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index - 2]), // handler
                    new (OpCodes.Ldarg_1), // target
                    new CodeInstruction(OpCodes.Ldloc_S, allow.LocalIndex), // allowed (bool)
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Attack), nameof(HandlerDamage)))
                });

            return list.AsEnumerable();
        }
    }
}