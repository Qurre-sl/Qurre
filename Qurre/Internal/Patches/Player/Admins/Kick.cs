using System;
using CommandSystem;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using Qurre.Loader;
using RemoteAdmin;

namespace Qurre.Internal.Patches.Player.Admins
{
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.KickUser), typeof(ReferenceHub), typeof(ICommandSender), typeof(string))]
    internal static class Kick
    {
        [HarmonyPrefix]
        private static bool Call(ReferenceHub target, ICommandSender issuer, string reason, ref bool __result)
        {
            __result = false;

            try
            {
                if (target is null)
                {
                    return false;
                }

                API.Player issue = null;

                if (issuer is PlayerCommandSender plsender)
                {
                    issue = plsender.ReferenceHub.GetPlayer();
                }
                else if (issuer is CommandSender sender)
                {
                    issue = sender.GetPlayer();
                }

                KickEvent ev = new (target.GetPlayer(), issue ?? API.Server.Host, reason);
                ev.InvokeEvent();

                if (!ev.Allowed)
                {
                    return false;
                }

                ServerConsole.Disconnect(target.gameObject, Configs.Kicked + reason);

                __result = true;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Admins}} [Kick]:{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}