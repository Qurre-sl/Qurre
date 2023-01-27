using HarmonyLib;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using System;

namespace Qurre.Internal.Patches.Map.Place
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
    static class RagdollSpawn
    {
        [HarmonyPrefix]
        static bool Call(ReferenceHub owner, DamageHandlerBase handler)
        {
            try
            {
                if (owner is null) return false;

                RagdollSpawnEvent ev = new(owner.GetPlayer(), handler);
                ev.InvokeEvent();

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Map> {{Place}} [RagdollSpawn]: {e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}