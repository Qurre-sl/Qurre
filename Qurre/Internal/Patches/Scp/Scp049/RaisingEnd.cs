namespace Qurre.Internal.Patches.Scp.Scp049;

using HarmonyLib;
using PlayerRoles.PlayableScps.Scp049;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

[HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerComplete))]
static class RaisingEnd
{
    [HarmonyPrefix]
    static bool Call(Scp049ResurrectAbility __instance)
    {
        if (__instance.CurRagdoll is null)
            return false;

        Player target = __instance.CurRagdoll.Info.OwnerHub.GetPlayer();
        if (target is null)
            return false;

        Scp049RaisingEndEvent @event = new(__instance.Owner.GetPlayer(), target, __instance.CurRagdoll);
        @event.InvokeEvent();

        return @event.Allowed;
    }
}
