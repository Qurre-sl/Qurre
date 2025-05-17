using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Admins;

[HarmonyPatch(typeof(BanHandler), nameof(BanHandler.IssueBan))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class Banned
{
    internal static readonly List<string> Cached = [];

    [HarmonyPostfix]
    private static void Call(BanDetails ban, BanHandler.BanType banType, bool forced, bool __result)
    {
        if (!__result)
            return;

        try
        {
            string _cache = ban.ToString();
            if (Cached.Contains(_cache))
                return;

            Cached.Add(_cache);

            BannedEvent @event = new(string.IsNullOrEmpty(ban.Id) ? null : ban.Id.GetPlayer(), ban, banType, forced);
            @event.InvokeEvent();

            if (!@event.UnsafeAllowed)
                BanHandler.RemoveBan(ban.Id, banType, true);
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Admins}} [Banned]: {e}\n{e.StackTrace}");
        }
    }
}