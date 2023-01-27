using HarmonyLib;
using PlayerStatsSystem;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

namespace Qurre.Internal.Patches.Player.Health
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.ProcessDamage))]
    static class Attack
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            Label metLabel = retLabel;
            {
                for (int iq = 0; iq < instructions.Count(); iq++)
                {
                    var ins = instructions.ElementAt(iq);
                    if (ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                        methodBase.Name == nameof(StandardDamageHandler.ProcessDamage))
                    {
                        metLabel = instructions.ElementAt(iq - 2).labels[0];
                    }
                }
            }

            LocalBuilder allow = generator.DeclareLocal(typeof(bool));

            List<CodeInstruction> list = new();

            int level = 0;
            for (int i = 0; i < instructions.Count(); i++)
            {
                var ins = instructions.ElementAt(i);

                // set "allowed = false" instant "ev.Damage = 0"
                if (level == 0 || level == 1)
                {
                    if (ins.opcode == OpCodes.Callvirt && ins.operand is not null && ins.operand is MethodBase methodBase &&
                        methodBase == AccessTools.PropertySetter(typeof(StandardDamageHandler), nameof(StandardDamageHandler.Damage)))
                    {
                        level++;

                        list.RemoveRange(i - 2, list.Count - (i - 2));
                        list.AddRange(new CodeInstruction[] // bool allowed = false; goto: method;
                        {
                            new CodeInstruction(OpCodes.Nop), // balance list count: 4 -> 4
                            new CodeInstruction(OpCodes.Ldc_I4_0),
                            new CodeInstruction(OpCodes.Stloc, allow.LocalIndex),
                            new CodeInstruction(OpCodes.Br, metLabel),
                        });
                        i++;
                        continue;
                    }
                }
                // remove "ev.Damage *= FFMultiplier"
                else if (level == 2)
                {
                    if (ins.opcode == OpCodes.Callvirt && ins.operand is not null && ins.operand is MethodBase methodBase &&
                        methodBase == AccessTools.PropertySetter(typeof(StandardDamageHandler), nameof(StandardDamageHandler.Damage)))
                    {
                        level++;
                        list.RemoveRange(i - 5, list.Count - (i - 5));
                        continue;
                    }
                }

                list.Add(ins);
            }

            list.InsertRange(0, new CodeInstruction[] // bool allowed = true;
            {
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Stloc, allow.LocalIndex),
            });

            int index = list.FindLastIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name == nameof(StandardDamageHandler.ProcessDamage));
            list.InsertRange(index == -1 ? list.Count - 3 : index - 2, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index - 2]), // handler
                new CodeInstruction(OpCodes.Ldarg_1), // target
                new CodeInstruction(OpCodes.Ldloc_S, allow.LocalIndex), // allowed (bool)
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Attack), nameof(Attack.HandlerDamage))),
            });

            return list.AsEnumerable();
        }

        static internal void HandlerDamage(AttackerDamageHandler handler, ReferenceHub target, bool allowed)
        {
            try
            {
                Player attacker = handler.Attacker.Hub.GetPlayer();
                if (attacker.FriendlyFire)
                    handler.IsFriendlyFire = false;

                AttackEvent ev = new(attacker, target.GetPlayer(), handler, handler.Damage, handler.IsFriendlyFire, allowed);
                ev.InvokeEvent();

                if (ev.Damage == -1)
                    ev.Damage = ev.Target.HealthInfomation.Hp + 1;

                handler.Damage = ev.Damage;
                handler.IsFriendlyFire = ev.FriendlyFire;

                if (!ev.Allowed)
                    handler.Damage = 0;

                if (ev.FriendlyFire)
                {
                    if (!Server.FriendlyFire) handler.Damage = 0;
                    else handler.Damage *= AttackerDamageHandler._ffMultiplier;
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Health}} [Attack]: {e}\n{e.StackTrace}");
            }
        }
    }
}