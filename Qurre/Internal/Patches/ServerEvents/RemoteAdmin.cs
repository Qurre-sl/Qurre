using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using RemoteAdmin;

namespace Qurre.Internal.Patches.ServerEvents;

[HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class RemoteAdmin
{
    [HarmonyPrefix]
    private static bool Call(string q, CommandSender sender)
    {
        try
        {
            IdleMode.PreauthStopwatch.Restart();
            IdleMode.SetIdleMode(false);

            if (q.StartsWith("$0 1"))
            {
                RequestPlayerListCommandEvent req = new(sender, sender.GetPlayer(), q);
                req.InvokeEvent();

                if (!string.IsNullOrEmpty(req.Reply))
                    sender.Print(req.Reply);

                return req.Allowed;
            }

            string[] arr = q.Split(' ');
            string name = arr[0].ToLower();
            string[] args = arr.Skip(1).ToArray();

            RemoteAdminCommandEvent ev = new(sender, sender.GetPlayer(), q, name, args);
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