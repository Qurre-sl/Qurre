using HarmonyLib;
using MapGeneration.Distributors;
using System;

namespace Qurre.Internal.Patches.Misc.Creates
{
    using Qurre.API;

    [HarmonyPatch(typeof(Locker), nameof(Locker.Start))]
    static class Locker1
    {
        [HarmonyPostfix]
        static void Call(Locker __instance)
        {
            try
            {
                Map.Lockers.RemoveAll(x => x.GameObject == null);
                Map.Lockers.Add(new(__instance));
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Misc> {{Creates}} [Locker]: {e}\n{e.StackTrace}");
            }
        }
    }
}