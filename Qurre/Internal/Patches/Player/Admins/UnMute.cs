using HarmonyLib;
using VoiceChat;
using System;

namespace Qurre.Internal.Patches.Player.Admins
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(VoiceChatMutes), nameof(VoiceChatMutes.RevokeLocalMute))]
    static class UnMute
    {
        [HarmonyPrefix]
        static bool Call(string userId, ref bool intercom)
        {
            try
            {
                Player pl = userId.GetPlayer();
                if (pl is null) return true;

                var ev = new UnMuteEvent(pl, intercom);
                ev.InvokeEvent();

                intercom = ev.Intercom;
                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Admins}} [UnMute]:{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}