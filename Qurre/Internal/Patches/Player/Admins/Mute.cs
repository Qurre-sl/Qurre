using System;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using VoiceChat;

namespace Qurre.Internal.Patches.Player.Admins
{
    [HarmonyPatch(typeof(VoiceChatMutes), nameof(VoiceChatMutes.IssueLocalMute))]
    internal static class Mute
    {
        [HarmonyPrefix]
        private static bool Call(string userId, ref bool intercom)
        {
            try
            {
                API.Player pl = userId.GetPlayer();

                if (pl is null)
                {
                    return true;
                }

                var ev = new MuteEvent(pl, intercom);
                ev.InvokeEvent();

                intercom = ev.Intercom;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Admins}} [Mute]:{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}