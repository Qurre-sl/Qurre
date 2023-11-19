using HarmonyLib;
using PlayerRoles.PlayableScps.Scp106;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Scp.Scp106
{
    [HarmonyPatch(typeof(Scp106Attack), nameof(Scp106Attack.ServerShoot))]
    static class Attack
    {
        [HarmonyPrefix]
        static bool Call(Scp106Attack __instance)
        {
            Scp106AttackEvent ev = new(__instance.Owner.GetPlayer(), __instance._targetHub.GetPlayer());
            ev.InvokeEvent();

            return ev.Allowed;
        }
    }
}