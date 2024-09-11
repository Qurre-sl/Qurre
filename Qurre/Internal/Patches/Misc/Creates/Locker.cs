using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API;

namespace Qurre.Internal.Patches.Misc.Creates;

[HarmonyPatch(typeof(Locker), nameof(Locker.Start))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class Locker1
{
    [HarmonyPostfix]
    private static void Call(Locker __instance)
    {
        try
        {
            Map.Lockers.RemoveAll(x => x.GameObject == null);
            Map.Lockers.Add(new API.Controllers.Locker(__instance));
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Misc> {{Creates}} [Locker]: {e}\n{e.StackTrace}");
        }
    }
}