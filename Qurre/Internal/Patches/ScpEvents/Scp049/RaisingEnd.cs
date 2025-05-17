using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp049;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.ScpEvents.Scp049;

[HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerComplete))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class RaisingEnd
{
    [HarmonyPrefix]
    private static bool Call(Scp049ResurrectAbility __instance)
    {
        if (__instance.CurRagdoll == null)
            return false;

        Player? target = __instance.CurRagdoll.Info.OwnerHub.GetPlayer();
        Player? player = __instance.Owner.GetPlayer();

        if (target is null || player is null)
            return false;

        Scp049RaisingEndEvent @event = new(player, target, __instance.CurRagdoll);
        @event.InvokeEvent();

        return @event.Allowed;
    }
}