using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using Console = GameCore.Console;

namespace Qurre.Internal.Patches.ServerEvents;

[HarmonyPatch(typeof(Console), nameof(Console.TypeCommand))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class ServerConsole
{
    [HarmonyPrefix]
    private static bool Call(string cmd, ref string __result)
    {
        try
        {
            string[] arr = cmd.Split(' ');
            string name = arr[0].ToLower();
            string[] args = arr.Skip(1).ToArray();

            ServerConsoleCommandEvent ev = new(cmd, name, args);
            ev.InvokeEvent();

            __result = ev.Reply;
            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Server> [ServerConsole]: {e}\n{e.StackTrace}");
            return true;
        }
    }
}