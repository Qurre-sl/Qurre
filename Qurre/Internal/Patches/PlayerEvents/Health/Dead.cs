using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PlayerRoles;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Objects;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Health;

[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class Dead
{
    [HarmonyPrefix]
    private static bool CallPre(PlayerStats __instance, DamageHandlerBase handler)
    {
        try
        {
            Player attacker = handler.GetAttacker() ?? Server.Host;
            Player? target = __instance.gameObject.GetPlayer();

            if (target is null)
                return true;

            DiesEvent ev = new(attacker, target, handler);
            ev.InvokeEvent();

            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Health}} [Dies]: {e}\n{e.StackTrace}");
        }

        return true;
    }

    [HarmonyPostfix]
    private static void Call(PlayerStats __instance, DamageHandlerBase handler)
    {
        try
        {
            Player? attacker = handler.GetAttacker();
            Player? target = __instance.gameObject.GetPlayer();

            attacker ??= target;

            if (target is null || attacker is null)
                return;

            if (target.RoleInformation.Role != RoleTypeId.Spectator ||
                target.GamePlay.GodMode || target.IsHost)
                return;

            DamageTypes type = handler.GetDamageType();
            DeadEvent ev = new(attacker, target, handler, type);
            ev.InvokeEvent();
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Health}} [Dead]: {e}\n{e.StackTrace}");
        }
    }
}