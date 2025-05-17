using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PlayerRoles.Ragdolls;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.World;

namespace Qurre.Internal.Patches.Misc.Creates;

[HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class DollPatch
{
    [HarmonyPostfix]
    private static void Call(ReferenceHub owner, BasicRagdoll __result)
    {
        try
        {
            if (__result == null)
                return;

            Corpse corpse = new(__result, owner.GetPlayer());
            Map.Corpses.Add(corpse);
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Misc> {{Creates}} [Doll]: {e}\n{e.StackTrace}");
        }
    }
}