using System;
using System.Collections.Generic;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Admins
{
    [HarmonyPatch(typeof(BanHandler), nameof(BanHandler.IssueBan))]
    internal static class Banned
    {
        internal static readonly List<string> Cached = new ();

        [HarmonyPostfix]
        private static void Call(BanDetails ban, BanHandler.BanType banType, bool __result)
        {
            if (!__result)
            {
                return;
            }

            try
            {
                var _cache = ban.ToString();

                if (Cached.Contains(_cache))
                {
                    _cache = null;
                    return;
                }

                Cached.Add(_cache);
                _cache = null;

                new BannedEvent(string.IsNullOrEmpty(ban.Id) ? null : ban.Id.GetPlayer(), ban, banType).InvokeEvent();
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Admins}} [Banned]:{e}\n{e.StackTrace}");
            }
        }
    }
}