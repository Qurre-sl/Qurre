using HarmonyLib;
using PlayerRoles.Spectating;

namespace Qurre.Internal.Patches.Player.Socialization
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(SpectatorRole), nameof(SpectatorRole.SyncedSpectatedNetId), MethodType.Setter)]
    static class ChangeSpectate
    {
        [HarmonyPrefix]
        static void Call(SpectatorRole __instance, ref uint value)
        {
            if (__instance._lastOwner is null)
                return;

            Player oldTarget = Server.Host;
            if (ReferenceHub.TryGetHubNetID(__instance.SyncedSpectatedNetId, out var oldTargetHub))
                oldTarget = oldTargetHub.GetPlayer() ?? Server.Host;

            Player newTarget = Server.Host;
            if (ReferenceHub.TryGetHubNetID(value, out var newTargetHub))
                newTarget = newTargetHub.GetPlayer() ?? Server.Host;

            var ev = new ChangeSpectateEvent(__instance._lastOwner.GetPlayer(), oldTarget, newTarget);
            ev.InvokeEvent();
        }
    }
}