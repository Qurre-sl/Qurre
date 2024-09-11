using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PlayerRoles.Spectating;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Socialization;

[HarmonyPatch(typeof(SpectatorRole), nameof(SpectatorRole.SyncedSpectatedNetId), MethodType.Setter)]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class ChangeSpectate
{
    [HarmonyPrefix]
    private static void Call(SpectatorRole __instance, ref uint value)
    {
        if (__instance._lastOwner == null)
            return;

        Player? player = __instance._lastOwner.GetPlayer();

        if (player is null)
            return;

        Player? oldTarget = null;
        if (ReferenceHub.TryGetHubNetID(__instance.SyncedSpectatedNetId, out ReferenceHub? oldTargetHub))
            oldTarget = oldTargetHub.GetPlayer();

        Player? newTarget = null;
        if (ReferenceHub.TryGetHubNetID(value, out ReferenceHub? newTargetHub))
            newTarget = newTargetHub.GetPlayer();

        ChangeSpectateEvent ev = new(player, oldTarget, newTarget);
        ev.InvokeEvent();
    }
}