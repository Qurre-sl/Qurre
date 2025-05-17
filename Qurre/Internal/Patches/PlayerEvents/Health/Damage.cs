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

namespace Qurre.Internal.Patches.PlayerEvents.Health;

[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.DealDamage))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Damage
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> list = [.. instructions];

        int index = list.FindIndex(ins =>
            ins.opcode == OpCodes.Callvirt && ins.operand is MethodBase
            {
                Name: nameof(DamageHandlerBase.ApplyDamage)
            });

        if (index < 3)
        {
            Log.Error($"Creating Patch error: <Player> {{Health}} [Damage]: Index - {index} < 3");
            return list.AsEnumerable();
        }

        Label contLabel = generator.DefineLabel();
        var labels = list[index - 3].ExtractLabels();
        list[index - 3].labels.Add(contLabel);

        list.InsertRange(index - 3,
        [
            new CodeInstruction(OpCodes.Ldarg_1).WithLabels(labels), // handler

            new CodeInstruction(OpCodes.Ldarg_0), // this
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PlayerStats), nameof(PlayerStats._hub))),

            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Damage), nameof(DamageCall))),
            new CodeInstruction(OpCodes.Brtrue, contLabel),

            new CodeInstruction(OpCodes.Ldc_I4_0),
            new CodeInstruction(OpCodes.Ret)
        ]);

        return list.AsEnumerable();
    }
    /*
     * ...
     * var @bool = Damage.DamageCall(handler, this._hub);
     * if (!@bool) return;
     * ...
     */

    internal static bool DamageCall(DamageHandlerBase handler, ReferenceHub hub)
    {
        try
        {
            float doDamage = GetDamage(handler);
            Player? attacker = null;
            Player? target = hub.GetPlayer();

            if (target is null || target.Disconnected)
                return true;

            if (handler is AttackerDamageHandler adh)
                attacker = adh.Attacker.Hub.GetPlayer();

            if (attacker is null)
            {
                if (handler is ExplosionDamageHandler)
                    return false;

                attacker = Server.Host;
            }

            DamageEvent ev = new(attacker, target, handler, doDamage);
            ev.InvokeEvent();

            if (!SetDamage(handler, ev.Damage))
                ev.Damage = doDamage;

            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Health}} [Damage]: {e}\n{e.StackTrace}");
            return true;
        }
    }

    private static float GetDamage(DamageHandlerBase handler)
    {
        return handler switch
        {
            StandardDamageHandler data => data.Damage,
            _ => -1
        };
    }

    private static bool SetDamage(DamageHandlerBase handler, float amount)
    {
        if (handler is not StandardDamageHandler data)
            return false;

        data.Damage = amount;
        return true;
    }
}