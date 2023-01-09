using HarmonyLib;
using Interactables.Interobjects;
using System;

namespace Qurre.Internal.Patches.Misc.Creates
{
    using Qurre.API;

    [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.Awake))]
    static class Lift
    {
        [HarmonyPostfix]
        static void Call(ElevatorChamber __instance)
        {
            try
            {
                Map.Lifts.RemoveAll(x => x.GameObject == null);
                Map.Lifts.Add(new(__instance));
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Misc> {{Creates}} [Lift]:{e}\n{e.StackTrace}");
            }
        }
    }
}