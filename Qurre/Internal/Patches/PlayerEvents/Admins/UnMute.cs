using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using VoiceChat;

namespace Qurre.Internal.Patches.PlayerEvents.Admins;

[HarmonyPatch(typeof(VoiceChatMutes), nameof(VoiceChatMutes.RevokeLocalMute))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class UnMute
{
    [HarmonyPrefix]
    private static bool Call(string userId, ref bool intercom)
    {
        try
        {
            Player? pl = userId.GetPlayer();

            if (pl is null)
                return true;

            UnMuteEvent ev = new(pl, intercom);
            ev.InvokeEvent();

            intercom = ev.Intercom;
            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Admins}} [UnMute]: {e}\n{e.StackTrace}");
            return true;
        }
    }
}