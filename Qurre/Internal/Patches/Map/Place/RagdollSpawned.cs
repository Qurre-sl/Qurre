using HarmonyLib;
using PlayerRoles.Ragdolls;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using System;

namespace Qurre.Internal.Patches.Map.Place
{
    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
    static class RagdollSpawned
    {
        [HarmonyPostfix]
        static void Call(BasicRagdoll __result)
        {
            try
            {
                if (__result is null)
                    return;

                RagdollSpawnedEvent ev = new(__result.GetRagdoll());
                ev.InvokeEvent();
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Map> {{Place}} [RagdollSpawned]: {e}\n{e.StackTrace}");
            }
        }
    }
}