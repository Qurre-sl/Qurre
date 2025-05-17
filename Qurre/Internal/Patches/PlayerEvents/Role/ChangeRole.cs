using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PlayerRoles;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using UnityEngine;

namespace Qurre.Internal.Patches.PlayerEvents.Role;

[HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.ServerSetRole))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class ChangeRole
{
    [HarmonyPrefix]
    private static bool Call(PlayerRoleManager __instance, ref RoleTypeId newRole, ref RoleChangeReason reason)
    {
        try
        {
            Player? pl = __instance.Hub.GetPlayer();

            if (pl is null)
                return true;

            if (pl.Disconnected)
                return true;

            ChangeRoleEvent ev = new(pl, __instance.CurrentRole, newRole, reason);
            ev.InvokeEvent();

            newRole = ev.Role;
            reason = ev.Reason;

            if (ev.Allowed)
                pl.LastSynced = Time.time;

            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Role}} [ChangeRole]: {e}\n{e.StackTrace}");
            return true;
        }
    }
}