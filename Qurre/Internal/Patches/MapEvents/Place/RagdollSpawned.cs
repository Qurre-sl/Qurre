﻿using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PlayerRoles.Ragdolls;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.MapEvents.Place;

[HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class RagdollSpawned
{
    [HarmonyPostfix]
    private static void Call(BasicRagdoll __result)
    {
        try
        {
            if (__result == null)
                return;

            new RagdollSpawnedEvent(__result.GetRagdoll()).InvokeEvent();
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Map> {{Place}} [RagdollSpawned]: {e}\n{e.StackTrace}");
        }
    }
}