using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Qurre.Internal.Patches.Player.Admins
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(BanHandler), nameof(BanHandler.IssueBan))]
    static class Banned
    {
        static internal readonly List<string> Cached = new();

        [HarmonyPostfix]
        static void Call(BanDetails ban, BanHandler.BanType banType, bool forced, bool __result)
        {
            if (!__result)
                return;

            try
            {
                string _cache = ban.ToString();
                if (Cached.Contains(_cache))
                {
                    _cache = null;
                    return;
                }
                Cached.Add(_cache);
                _cache = null;

                BannedEvent @event = new(string.IsNullOrEmpty(ban.Id) ? null : ban.Id.GetPlayer(), ban, banType, forced);
                @event.InvokeEvent();

                if (!@event.UnsafeAllowed)
                {
                    BanHandler.RemoveBan(ban.Id, banType, forced: true);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Admins}} [Banned]: {e}\n{e.StackTrace}");
            }
        }
    }
}