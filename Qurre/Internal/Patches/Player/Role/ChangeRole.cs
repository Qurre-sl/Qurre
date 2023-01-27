using HarmonyLib;
using PlayerRoles;
using System;

namespace Qurre.Internal.Patches.Player.Role
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.ServerSetRole))]
    static class ChangeRole
    {
        [HarmonyPrefix]
        static bool Call(PlayerRoleManager __instance, ref RoleTypeId newRole, ref RoleChangeReason reason)
        {
            try
            {
                var pl = __instance.Hub.GetPlayer();

                if (pl is null)
                    return true;

                if (pl.Disconnected)
                    return true;

                ChangeRoleEvent ev = new(pl, __instance.CurrentRole, newRole, reason);
                ev.InvokeEvent();

                newRole = ev.Role;
                reason = ev.Reason;

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Role}} [ChangeRole]: {e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}