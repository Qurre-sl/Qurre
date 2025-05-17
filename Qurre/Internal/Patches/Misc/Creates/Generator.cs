using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API;
using Qurre.API.World;

namespace Qurre.Internal.Patches.Misc.Creates;

[HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.Start))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class Generator
{
    [HarmonyPostfix]
    private static void Call(Scp079Generator __instance)
    {
        try
        {
            Map.Generators.RemoveAll(x => x.GameObject == null);
            Map.Generators.Add(new API.Controllers.Generator(__instance));
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Misc> {{Creates}} [Generator]: {e}\n{e.StackTrace}");
        }
    }
}