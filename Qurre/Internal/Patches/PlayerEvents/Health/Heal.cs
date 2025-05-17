using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using UnityEngine;

namespace Qurre.Internal.Patches.PlayerEvents.Health;

[HarmonyPatch(typeof(HealthStat), nameof(HealthStat.ServerHeal))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Heal
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // instance [HealthStat]
        yield return new CodeInstruction(OpCodes.Ldarg_1); // healAmount [float]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Heal), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static void Invoke(HealthStat instance, float amount)
    {
        try
        {
            Player? player = instance.Hub.GetPlayer();

            if (player is null)
                return;

            HealEvent ev = new(player, amount);
            ev.InvokeEvent();

            if (!ev.Allowed)
                return;

            instance.CurValue = Mathf.Min(instance.CurValue + Mathf.Abs(ev.Amount),
                ev.Player.HealthInformation.MaxHp);
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Health}} [Heal]: {e}\n{e.StackTrace}");
        }
    }
}