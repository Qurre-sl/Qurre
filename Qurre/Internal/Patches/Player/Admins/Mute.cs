using HarmonyLib;
using VoiceChat;
using System;

namespace Qurre.Internal.Patches.Player.Admins
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(VoiceChatMutes), nameof(VoiceChatMutes.IssueLocalMute))]
    static class Mute
    {
        [HarmonyPrefix]
        static bool Call(string userId, ref bool intercom)
        {
            try
            {
                Player pl = userId.GetPlayer();
                if (pl is null) return true;

                var ev = new MuteEvent(pl, intercom);
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
}