using Mirror;
using PlayerRoles.Voice;
using BaseIntercom = PlayerRoles.Voice.Intercom;

namespace Qurre.API.Controllers
{
    public static class Intercom
    {
        public static IntercomDisplay Display => IntercomDisplay._singleton;
        public static BaseIntercom Base => BaseIntercom._singleton;

        public static Player Speaker => Base._curSpeaker.GetPlayer();

        public static string Text
        {
            get => Display._overrideText;
            set => Display._overrideText = value;
        }

        public static IntercomState Status
        {
            get => BaseIntercom.State;
            set => BaseIntercom.State = value;
        }

        public static float RechargeCooldown
        {
            get => Base._cooldownTime;
            set => Base._cooldownTime = value;
        }

        public static float SpeechRemaining
        {
            get => Base.RemainingTime;
            set => Base._nextTime = NetworkTime.time + value;
        }
    }
}