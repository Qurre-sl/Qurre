using Mirror;
using PlayerRoles.Voice;
using BaseIntercom = PlayerRoles.Voice.Intercom;

namespace Qurre.API.Controllers
{
    static public class Intercom
    {
        static public IntercomDisplay Display => IntercomDisplay._singleton;
        static public BaseIntercom Base => BaseIntercom._singleton;

        static public Player Speaker => Base._curSpeaker.GetPlayer();

        static public string Text
        {
            get => Display._overrideText;
            set => Display._overrideText = value;
        }
        static public IntercomState Status
        {
            get => BaseIntercom.State;
            set => BaseIntercom.State = value;
        }
        static public float RechargeCooldown
        {
            get => Base._cooldownTime;
            set => Base._cooldownTime = value;
        }
        static public float SpeechRemaining
        {
            get => Base.RemainingTime;
            set => Base._nextTime = NetworkTime.time + value;
        }
    }
}