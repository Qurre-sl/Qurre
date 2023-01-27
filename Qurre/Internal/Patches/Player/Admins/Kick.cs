using HarmonyLib;
using CommandSystem;
using RemoteAdmin;
using System;

namespace Qurre.Internal.Patches.Player.Admins
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.KickUser), new[] { typeof(ReferenceHub), typeof(ICommandSender), typeof(string) })]
    static class Kick
    {
        [HarmonyPrefix]
        static bool Call(ReferenceHub target, ICommandSender issuer, string reason, ref bool __result)
        {
            __result = false;
            try
            {
                if (target is null) return false;

                Player issue = null;
                if (issuer is PlayerCommandSender plsender)
                    issue = plsender.ReferenceHub.GetPlayer();
                else if (issuer is CommandSender sender)
                    issue = sender.GetPlayer();

                KickEvent ev = new(target.GetPlayer(), issue ?? Server.Host, reason);
                ev.InvokeEvent();

                if (!ev.Allowed) return false;

                ServerConsole.Disconnect(target.gameObject, Qurre.Loader.Configs.Kicked + reason);

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
}