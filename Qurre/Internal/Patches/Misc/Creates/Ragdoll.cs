using System;
using HarmonyLib;
using PlayerRoles.Ragdolls;
using Qurre.API;

namespace Qurre.Internal.Patches.Misc.Creates
{
    using Controller = API.Controllers.Ragdoll;

    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
    internal static class Ragdoll
    {
        [HarmonyPostfix]
        private static void Call(ReferenceHub owner, BasicRagdoll __result)
        {
            try
            {
                if (__result is null)
                {
                    return;
                }

                Controller ragdoll = new (__result, owner.GetPlayer());
                API.Map.Ragdolls.Add(ragdoll);
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Misc> {{Creates}} [Ragdoll]:{e}\n{e.StackTrace}");
            }
        }
    }
}