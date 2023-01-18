using System;
using HarmonyLib;
using Interactables.Interobjects;
using Qurre.API;

namespace Qurre.Internal.Patches.Misc.Creates
{
    [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.Awake))]
    internal static class Lift
    {
        [HarmonyPostfix]
        private static void Call(ElevatorChamber __instance)
        {
            try
            {
                API.Map.Lifts.RemoveAll(x => x.GameObject == null);
                API.Map.Lifts.Add(new (__instance));
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Misc> {{Creates}} [Lift]: {e}\n{e.StackTrace}");
            }
        }
    }
}