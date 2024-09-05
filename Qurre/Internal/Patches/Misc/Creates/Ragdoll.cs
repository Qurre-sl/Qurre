using HarmonyLib;
using PlayerRoles.Ragdolls;
using System;

namespace Qurre.Internal.Patches.Misc.Creates
{
    using Qurre.API;
    using Controller = API.Controllers.Ragdoll;

    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
    static class Ragdoll
    {
        [HarmonyPostfix]
        static void Call(ReferenceHub owner, BasicRagdoll __result)
        {
            try
            {
                if (__result is null)
                    return;

                Controller ragdoll = new(__result, owner.GetPlayer());
                Map.Ragdolls.Add(ragdoll);
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Misc> {{Creates}} [Ragdoll]: {e}\n{e.StackTrace}");
            }
        }
    }
}