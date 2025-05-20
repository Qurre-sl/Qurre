using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using RemoteAdmin;

namespace Qurre.Internal.Patches.ServerEvents;

[HarmonyPatch(typeof(QueryProcessor), nameof(QueryProcessor.ProcessGameConsoleQuery))]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class GameConsole
{
    [HarmonyPrefix]
    private static bool Call(QueryProcessor __instance, string query)
    {
        try
        {
            string[] arr = query.Split(' ');
            string name = arr[0].ToLower();
            string[] args = arr.Skip(1).ToArray();

            GameConsoleCommandEvent ev = new(__instance.gameObject.GetPlayer(), query, name, args);
            ev.InvokeEvent();

            if (!string.IsNullOrEmpty(ev.Reply))
                ev.Player.Client.SendConsole(ev.Reply, ev.Color);

            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Server> [GameConsole]: {e}\n{e.StackTrace}");
            return true;
        }
    }
}