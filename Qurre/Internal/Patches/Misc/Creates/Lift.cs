using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Interactables.Interobjects;
using Qurre.API;
using Qurre.API.World;

namespace Qurre.Internal.Patches.Misc.Creates;

[HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.Awake))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class Lift
{
    [HarmonyPostfix]
    private static void Call(ElevatorChamber __instance)
    {
        try
        {
            Map.Lifts.RemoveAll(x => x.GameObject == null);
            Map.Lifts.Add(new API.Controllers.Lift(__instance));
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Misc> {{Creates}} [Lift]: {e}\n{e.StackTrace}");
        }
    }
}