using System;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using VoiceChat;

namespace Qurre.Internal.Patches.Player.Admins
{
    [HarmonyPatch(typeof(VoiceChatMutes), nameof(VoiceChatMutes.RevokeLocalMute))]
    internal static class UnMute
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

                var ev = new UnMuteEvent(pl, intercom);
                ev.InvokeEvent();

                intercom = ev.Intercom;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Admins}} [UnMute]:{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}