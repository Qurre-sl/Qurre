using System;
using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API;

namespace Qurre.Internal.Patches.Misc.Creates
{
    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.Start))]
    internal static class Generator
    {
        [HarmonyPostfix]
        private static void Call(Scp079Generator __instance)
        {
            try
            {
                API.Map.Generators.RemoveAll(x => x.GameObject == null);
                API.Map.Generators.Add(new (__instance));
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Misc> {{Creates}} [Generator]:{e}\n{e.StackTrace}");
            }
        }
    }
}