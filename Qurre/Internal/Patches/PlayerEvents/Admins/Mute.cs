using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using VoiceChat;

namespace Qurre.Internal.Patches.PlayerEvents.Admins;

[HarmonyPatch(typeof(VoiceChatMutes), nameof(VoiceChatMutes.IssueLocalMute))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Mute
{
    [HarmonyPrefix]
    private static bool Call(string userId, ref bool intercom)
    {
        try
        {
            Player? pl = userId.GetPlayer();

            if (pl is null)
                return true;

            MuteEvent ev = new(pl, intercom);
            ev.InvokeEvent();

            intercom = ev.Intercom;
            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Admins}} [Mute]: {e}\n{e.StackTrace}");
            return true;
        }
    }
}