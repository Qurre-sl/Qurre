using System;
using JetBrains.Annotations;
using Mirror;
using PlayerRoles.Voice;
using Qurre.API.Controllers;
using BaseIntercom = PlayerRoles.Voice.Intercom;

namespace Qurre.API.World;

[PublicAPI]
public static class Intercom
{
    public static IntercomDisplay Display => IntercomDisplay._singleton;
    public static BaseIntercom Base => BaseIntercom._singleton;

    public static Player? Speaker => Base._curSpeaker.GetPlayer();

    public static string Text
    {
        get => Display._overrideText;
        set => Display.Network_overrideText = value;
    }

    public static IntercomState Status
    {
        get => BaseIntercom.State;
        set => BaseIntercom.State = value;
    }

    public static double RemainingCooldown
    {
        get => Status == IntercomState.Cooldown ? Math.Max(Base._nextTime - NetworkTime.time, 0) : 0;
        set => Base._nextTime = value + NetworkTime.time;
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