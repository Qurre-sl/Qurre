using System;
using HarmonyLib;
using PlayerRoles;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Role
{
    [HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.ServerSetRole))]
    internal static class ChangeRole
    {
        [HarmonyPrefix]
        private static bool Call(PlayerRoleManager __instance, ref RoleTypeId newRole, ref RoleChangeReason reason)
        {
            try
            {
                ChangeRoleEvent ev = new (__instance.Hub.GetPlayer(), __instance.CurrentRole, newRole, reason);
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