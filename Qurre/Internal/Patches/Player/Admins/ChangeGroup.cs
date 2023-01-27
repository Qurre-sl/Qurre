using HarmonyLib;
using System;

namespace Qurre.Internal.Patches.Player.Admins
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SetGroup))]
    static class ChangeGroup
    {
        [HarmonyPrefix]
        static bool Call(ServerRoles __instance, ref UserGroup group)
        {
            try
            {
                ChangeGroupEvent ev = new(__instance._hub.GetPlayer(), group);
                ev.InvokeEvent();

                group = ev.Group;

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Admins}} [ChangeGroup]: {e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}