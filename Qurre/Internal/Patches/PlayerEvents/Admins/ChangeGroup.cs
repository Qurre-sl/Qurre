using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Admins;

[HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SetGroup))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class ChangeGroup
{
    [HarmonyPrefix]
    private static bool Call(ServerRoles __instance, ref UserGroup group)
    {
        try
        {
            Player? player = __instance._hub.GetPlayer();

            if (player is null)
                return true;

            ChangeGroupEvent ev = new(player, group);
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