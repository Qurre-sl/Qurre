using HarmonyLib;
using Mirror;
using System;
using System.Linq;

namespace Qurre.Internal.Patches.Player.Network
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(CustomNetworkManager), nameof(CustomNetworkManager.OnServerDisconnect), new[] { typeof(NetworkConnectionToClient) })]
    static class Leave
    {
        [HarmonyPrefix]
        static void Call(NetworkConnection conn)
        {
            try
            {
                if (conn.identity is null) return;
                Player player = null;
                try { player = conn.identity.gameObject.GetPlayer(); } catch { }
                if (player is null || player.IsHost) return;

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
    static class Leaved
    {
        [HarmonyPrefix]
        static void Call(ReferenceHub __instance)
        {
            try
            {
                if (__instance.GetPlayer() is not Player player || player.IsHost)
                    return;

                if (Fields.Player.Dictionary.ContainsKey(player.GameObject))
                    Fields.Player.Dictionary.Remove(player.GameObject);

                if (Fields.Player.IDs.ContainsKey(player.UserInformation.Id))
                    Fields.Player.IDs.Remove(player.UserInformation.Id);

                if (Fields.Player.UserIDs.ContainsKey(player.UserInformation.UserId))
                    Fields.Player.UserIDs.Remove(player.UserInformation.UserId);

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
}