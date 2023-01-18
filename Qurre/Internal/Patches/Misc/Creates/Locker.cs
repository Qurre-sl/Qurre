using System;
using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API;

namespace Qurre.Internal.Patches.Misc.Creates
{
    [HarmonyPatch(typeof(Locker), nameof(Locker.Start))]
    internal static class Locker1
    {
        [HarmonyPostfix]
        private static void Call(Locker __instance)
        {
            try
            {
                API.Map.Lockers.RemoveAll(x => x.GameObject == null);
                API.Map.Lockers.Add(new (__instance));
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Misc> {{Creates}} [Locker]:{e}\n{e.StackTrace}");
            }
        }
    }
}