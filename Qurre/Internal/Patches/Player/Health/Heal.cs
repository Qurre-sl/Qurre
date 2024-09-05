using HarmonyLib;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace Qurre.Internal.Patches.Player.Health
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(HealthStat), nameof(HealthStat.ServerHeal))]
    static class Heal
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0); // instance [HealthStat]
            yield return new CodeInstruction(OpCodes.Ldarg_1); // healAmount [float]
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Heal), nameof(Heal.Invoke)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        static void Invoke(HealthStat instance, float amount)
        {
            try
            {
                HealEvent ev = new(instance.Hub.GetPlayer(), amount);
                ev.InvokeEvent();

                if (!ev.Allowed)
                    return;

                instance.CurValue = Mathf.Min(instance.CurValue + Mathf.Abs(ev.Amount), ev.Player?.HealthInformation?.MaxHp ?? instance.MaxValue);
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Health}} [Heal]: {e}\n{e.StackTrace}");
            }
        }
    }
}