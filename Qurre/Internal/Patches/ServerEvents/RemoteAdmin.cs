using System;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using RemoteAdmin;

namespace Qurre.Internal.Patches.ServerEvents;

[HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
internal static class RemoteAdmin
{
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Call(string q, CommandSender sender)
    {
        try
        {
            IdleMode.PreauthStopwatch.Restart();
            IdleMode.SetIdleMode(false);

            if (q.StartsWith("$0 1"))
            {
                RequestPlayerListCommandEvent requestPlayerListEv = new(sender, sender.GetPlayer(), q);
                requestPlayerListEv.InvokeEvent();

                if (!string.IsNullOrEmpty(requestPlayerListEv.Reply))
                    sender.Print(requestPlayerListEv.Reply);

                return requestPlayerListEv.Allowed;
            }

            if (q.StartsWith("$"))
                return true;

            string[]? arr = q.Split(' ');
            string commandName = arr[0].ToLower();
            string[] commandArgs = arr.Skip(1).ToArray();

            RemoteAdminCommandEvent ev = new(sender, sender.GetPlayer(), q, commandName, commandArgs);
            ev.InvokeEvent();

            if (!string.IsNullOrEmpty(ev.Reply))
                sender.RaReply($"{ev.Prefix}#{ev.Reply}", ev.Success, true, string.Empty);

            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Server> [RemoteAdmin]: {e}\n{e.StackTrace}");
            return true;
        }
    }
}