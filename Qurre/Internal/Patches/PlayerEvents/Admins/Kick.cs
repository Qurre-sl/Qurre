using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using Qurre.Loader;
using RemoteAdmin;

namespace Qurre.Internal.Patches.PlayerEvents.Admins;

[HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.KickUser), typeof(ReferenceHub), typeof(ICommandSender),
    typeof(string))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class Kick
{
    [HarmonyPrefix]
    private static bool Call(ReferenceHub target, ICommandSender issuer, string reason, ref bool __result)
    {
        try
        {
            Player? targetPlayer = target.GetPlayer();

            if (targetPlayer is null)
                return false;

            Player? issue = issuer switch
            {
                PlayerCommandSender plSender => plSender.ReferenceHub.GetPlayer(),
                CommandSender sender => sender.GetPlayer(),
                _ => null
            };

            KickEvent ev = new(targetPlayer, issue ?? Server.Host, reason);
            ev.InvokeEvent();

            if (!ev.Allowed)
                return false;

            ServerConsole.Disconnect(targetPlayer.GameObject, Configs.Kicked + reason);

            __result = true;
            return false;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Admins}} [Kick]: {e}\n{e.StackTrace}");
            return true;
        }
    }
}