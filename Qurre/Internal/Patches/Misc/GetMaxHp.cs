using HarmonyLib;
using PlayerStatsSystem;
using System;

namespace Qurre.Internal.Patches.Misc
{
    using Qurre.API;

    [HarmonyPatch(typeof(HealthStat), nameof(HealthStat.MaxValue), MethodType.Getter)]
    static class GetMaxHp
    {
        [HarmonyPrefix]
        static bool Call(HealthStat __instance, ref float __result)
        {
            try
            {
                var pl = __instance.Hub.GetPlayer();

                if (pl is null)
                    return true;

                if (pl.HealthInformation._maxHp < 1)
                    return true;

                __result = pl.HealthInformation._maxHp;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Misc> [GetMaxHp]: {e}\n{e.StackTrace}");
                return true;
            }
        }

        [HarmonyPostfix]
        static void FixZeroHp(HealthStat __instance, ref float __result)
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
}