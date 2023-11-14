using Mirror;
using PlayerRoles.Voice;
using System;
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
            set => Display.Network_overrideText = value;
        }
        static public IntercomState Status
        {
            get => BaseIntercom.State;
            set => BaseIntercom.State = value;
        }
        static public double RemainingCooldown
        {
            get => Status == IntercomState.Cooldown ? Math.Max(Base._nextTime - NetworkTime.time, 0) : 0;
            set => Base._nextTime = value + NetworkTime.time;
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