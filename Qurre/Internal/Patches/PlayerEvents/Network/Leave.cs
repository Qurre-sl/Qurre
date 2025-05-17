using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Network;

[HarmonyPatch(typeof(CustomNetworkManager), nameof(CustomNetworkManager.OnServerDisconnect),
    typeof(NetworkConnectionToClient))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Leave
{
    [HarmonyPrefix]
    private static void Call(NetworkConnection conn)
    {
        try
        {
            if (conn.identity == null)
                return;

            Player? player = conn.identity?.gameObject.GetPlayer();

            if (player is null || player.IsHost)
                return;

            ServerConsole.AddLog(
                $"Player {player.UserInformation.Nickname} ({player.UserInformation.UserId}) ({player.UserInformation.Id}) disconnected",
                ConsoleColor.DarkMagenta
            );

            new LeaveEvent(player).InvokeEvent();

            player.Disconnected = true;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Network}} [Leave]: {e}\n{e.StackTrace}");
        }
    }
}

[HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class Leaved
{
    [HarmonyPrefix]
    private static void Call(ReferenceHub __instance)
    {
        try
        {
            if (__instance.GetPlayer() is not { } player || player.IsHost)
                return;

            Fields.Player.Dictionary.Remove(player.GameObject);
            Fields.Player.Hubs.Remove(player.ReferenceHub);
            Fields.Player.Ids.Remove(player.UserInformation.Id);

            foreach (var item in Fields.Player.Args.Where(kvp => kvp.Value == player).ToList())
                Fields.Player.Args.Remove(item.Key);

            player.Disconnected = true;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Network}} [Leaved]: {e}\n{e.StackTrace}");
        }
    }
}