using System;
using JetBrains.Annotations;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class BanEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Ban;

    internal BanEvent(Player player, Player issuer, DateTime expires, string reason)
    {
        Player = player;
        Issuer = issuer;
        Expires = expires;
        Reason = reason;
        Allowed = true;
    }

    public Player Player { get; }
    public Player Issuer { get; }
    public DateTime Expires { get; set; }
    public string Reason { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class BannedEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Banned;

    internal BannedEvent(Player? player, BanDetails details, BanHandler.BanType type, bool forced)
    {
        Player = player;
        Details = details;
        Type = type;
        Forced = forced;
        UnsafeAllowed = true;
    }

    public Player? Player { get; }
    public BanDetails Details { get; }
    public BanHandler.BanType Type { get; }
    public bool Forced { get; }
    public bool UnsafeAllowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class KickEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Kick;

    internal KickEvent(Player player, Player issuer, string reason)
    {
        Player = player;
        Issuer = issuer;
        Reason = reason;
        Allowed = true;
    }

    public Player Player { get; }
    public Player Issuer { get; }
    public string Reason { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class MuteEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Mute;

    internal MuteEvent(Player player, bool icom)
    {
        Player = player;
        Intercom = icom;
        Allowed = true;
    }

    public Player Player { get; }
    public bool Intercom { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class UnMuteEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Unmute;

    internal UnMuteEvent(Player player, bool icom)
    {
        Player = player;
        Intercom = icom;
        Allowed = true;
    }

    public Player Player { get; }
    public bool Intercom { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class ChangeGroupEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.ChangeGroup;

    internal ChangeGroupEvent(Player player, UserGroup group)
    {
        Player = player;
        Group = group;
        Allowed = true;
    }

    public Player Player { get; }
    public UserGroup Group { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}