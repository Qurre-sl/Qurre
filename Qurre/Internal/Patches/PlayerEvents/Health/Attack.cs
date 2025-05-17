using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using UnityEngine;

namespace Qurre.Internal.Patches.PlayerEvents.Health;

[HarmonyPatch(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.ProcessDamage))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Attack
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> ins, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        List<CodeInstruction> instructions = [.. ins];
        instructions.Last().labels.Add(retLabel);

        Label metLabel = retLabel;
        {
            for (int iq = 0; iq < instructions.Count; iq++)
            {
                CodeInstruction value = instructions.ElementAt(iq);
                if (value.opcode == OpCodes.Call && value.operand is MethodBase
                    {
                        Name: nameof(StandardDamageHandler.ProcessDamage)
                    })
                    metLabel = instructions[iq - 2].labels[0];
            }
        }

        LocalBuilder allow = generator.DeclareLocal(typeof(bool));

        List<CodeInstruction> list = [];

        int level = 0;
        for (int i = 0; i < instructions.Count(); i++)
        {
            CodeInstruction value = instructions.ElementAt(i);

            switch (level)
            {
                // set "allowed = false" instant "ev.Damage = 0"
                case 0 or 1 when value.opcode == OpCodes.Callvirt && value.operand is MethodBase
                {
                    Name: nameof(StandardDamageHandler.Damage)
                }:
                    level++;

                    list.RemoveRange(i - 2, list.Count - (i - 2));
                    list.AddRange( // bool allowed = false; goto: method;
                    [
                        new CodeInstruction(OpCodes.Nop), // balance list count: 4 -> 4
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Stloc, allow.LocalIndex),
                        new CodeInstruction(OpCodes.Br, metLabel)
                    ]);
                    i++;
                    continue;
                // remove "ev.Damage *= FFMultiplier"
                case 2 when value.opcode == OpCodes.Callvirt && value.operand is MethodBase
                {
                    Name: nameof(StandardDamageHandler.Damage)
                }:
                    level++;
                    list.RemoveRange(i - 5, list.Count - (i - 5));
                    continue;
                default:
                    list.Add(value);
                    break;
            }
        }

        list.InsertRange(0, // bool allowed = true;
        [
            new CodeInstruction(OpCodes.Ldc_I4_1),
            new CodeInstruction(OpCodes.Stloc, allow.LocalIndex)
        ]);

        int index = list.FindLastIndex(x => x.opcode == OpCodes.Call && x.operand is MethodBase
        {
            Name: nameof(StandardDamageHandler.ProcessDamage)
        });
        list.InsertRange(index == -1 ? list.Count - 3 : index - 2,
        [
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index - 2]), // handler
            new CodeInstruction(OpCodes.Ldarg_1), // target
            new CodeInstruction(OpCodes.Ldloc_S, allow.LocalIndex), // allowed (bool)
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Attack), nameof(HandlerDamage)))
        ]);

        return list.AsEnumerable();
    }

    internal static void HandlerDamage(AttackerDamageHandler? handler, ReferenceHub? targetHub, bool allowed)
    {
        try
        {
            if (handler is null)
                return;

            if (targetHub == null)
                return;

            Player? attacker = handler.Attacker.Hub.GetPlayer();
            Player? target = targetHub.GetPlayer();

            if (attacker is null || target is null)
                return;

            if (attacker.FriendlyFire)
                handler.IsFriendlyFire = false;

            AttackEvent ev = new(attacker, target, handler, handler.Damage, handler.IsFriendlyFire,
                allowed);
            ev.InvokeEvent();

            if (Mathf.Approximately(ev.Damage, -1))
                ev.Damage = ev.Target.HealthInformation.Hp + 1;

            handler.Damage = ev.Damage;
            handler.IsFriendlyFire = ev.FriendlyFire;

            if (!ev.Allowed)
                handler.Damage = 0;

            if (!ev.FriendlyFire)
                return;

            if (!Server.FriendlyFire)
                handler.Damage = 0;
            else
                handler.Damage *= AttackerDamageHandler._ffMultiplier;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Health}} [Attack]: {e}\n{e.StackTrace}");
        }
    }
}