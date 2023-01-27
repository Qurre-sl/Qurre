using HarmonyLib;
using MapGeneration.Distributors;
using System;

namespace Qurre.Internal.Patches.Misc.Creates
{
    using Qurre.API;

    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.Start))]
    static class Generator
    {
        [HarmonyPostfix]
        static void Call(Scp079Generator __instance)
        {
            try
            {
                Map.Generators.RemoveAll(x => x.GameObject == null);
                Map.Generators.Add(new(__instance));
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Misc> {{Creates}} [Generator]: {e}\n{e.StackTrace}");
            }
        }
    }
}