using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Controllers;

namespace Qurre.Internal.Patches.Misc;

[HarmonyPatch(typeof(HealthStat), nameof(HealthStat.MaxValue), MethodType.Getter)]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class GetMaxHp
{
    [HarmonyPrefix]
    private static bool Call(HealthStat __instance, ref float __result)
    {
        try
        {
            Player? pl = __instance.Hub.GetPlayer();

            if (pl is null)
                return true;

            if (pl.HealthInformation.LocalMaxHp < 1)
                return true;

            __result = pl.HealthInformation.LocalMaxHp;
            return false;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Misc> [GetMaxHp]: {e}\n{e.StackTrace}");
            return true;
        }
    }

    [HarmonyPostfix]
    private static void FixZeroHp(ref float __result)
    {
        try
        {
            if (__result > 0)
                return;

            __result = 100;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Misc> [GetMaxHp.FixZeroHp]: {e}\n{e.StackTrace}");
        }
    }
}