using HarmonyLib;
using System;
using System.Linq;

namespace Qurre.Internal.Patches.Server
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(GameCore.Console), nameof(GameCore.Console.TypeCommand))]
    static class ServerConsole
    {
        [HarmonyPrefix]
        static bool Call(string cmd, ref string __result)
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
}