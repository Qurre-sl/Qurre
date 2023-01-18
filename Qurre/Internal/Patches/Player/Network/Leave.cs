﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Network
{
    [HarmonyPatch(typeof(CustomNetworkManager), nameof(CustomNetworkManager.OnServerDisconnect), typeof(NetworkConnection))]
    internal static class Leave
    {
        [HarmonyPrefix]
        private static void Call(NetworkConnection conn)
        {
            try
            {
                if (conn.identity is null)
                {
                    return;
                }

                API.Player player = null;

                try
                {
                    player = conn.identity.gameObject.GetPlayer();
                }
                catch { }

                if (player is null || player.IsHost)
                {
                    return;
                }

                ServerConsole.AddLog(
                    $"Player {player.UserInfomation.Nickname} ({player.UserInfomation.UserId}) ({player.UserInfomation.Id}) disconnected",
                    ConsoleColor.DarkMagenta
                );

                new LeaveEvent(player).InvokeEvent();
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Network}} [Leave]:{e}\n{e.StackTrace}");
            }
        }
    }

    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    internal static class Leaved
    {
        [HarmonyPrefix]
        private static void Call(ReferenceHub __instance)
        {
            try
            {
                if (__instance.GetPlayer() is not API.Player player || player.IsHost)
                {
                    return;
                }

                if (Fields.Player.Dictionary.ContainsKey(player.GameObject))
                {
                    Fields.Player.Dictionary.Remove(player.GameObject);
                }

                if (Fields.Player.IDs.ContainsKey(player.UserInfomation.Id))
                {
                    Fields.Player.IDs.Remove(player.UserInfomation.Id);
                }

                if (Fields.Player.UserIDs.ContainsKey(player.UserInfomation.UserId))
                {
                    Fields.Player.UserIDs.Remove(player.UserInfomation.UserId);
                }

                foreach (KeyValuePair<string, API.Player> item in Fields.Player.Args.Where(kvp => kvp.Value == player).ToList())
                {
                    Fields.Player.Args.Remove(item.Key);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Network}} [Leaved]:{e}\n{e.StackTrace}");
            }
        }
    }
}