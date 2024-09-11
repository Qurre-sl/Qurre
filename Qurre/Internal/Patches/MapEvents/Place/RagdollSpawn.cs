using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.MapEvents.Place;

[HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class RagdollSpawn
{
    [HarmonyPrefix]
    private static bool Call(ReferenceHub owner, DamageHandlerBase handler)
    {
        try
        {
            Player? player = owner.GetPlayer();

            if (player is null)
                return false;

            RagdollSpawnEvent ev = new(player, handler);
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