using System;
using JetBrains.Annotations;
using Qurre.API.Entities.Characters;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class BanEvent : ICancellableEvent
{
    internal BanEvent(Player player, Player issuer, DateTime expires, string reason)
    {
        Player = player;
        Issuer = issuer;
        Expires = expires;
        Reason = reason;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Ban;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public Player Issuer { get; }
    public DateTime Expires { get; set; }
    public string Reason { get; set; }
}

[PublicAPI]
public class BannedEvent : ICancellableEvent
{
    private const uint EventID = PlayerEvents.Banned;

    internal BannedEvent(Player? player, BanDetails details, BanHandler.BanType type, bool forced)
    {
        Player = player;
        Details = details;
        Type = type;
        Forced = forced;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = EventID;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true; // TODO: unsafe

    public Player? Player { get; }
    public BanDetails Details { get; }
    public BanHandler.BanType Type { get; }
    public bool Forced { get; }
}

[PublicAPI]
public class KickEvent : ICancellableEvent
{
    internal KickEvent(Player player, Player issuer, string reason)
    {
        Player = player;
        Issuer = issuer;
        Reason = reason;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Kick;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public Player Issuer { get; }
    public string Reason { get; set; }
}

[PublicAPI]
public class MuteEvent : ICancellableEvent
{
    internal MuteEvent(Player player, bool intercom)
    {
        Player = player;
        Intercom = intercom;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Mute;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public bool Intercom { get; set; }
}

[PublicAPI]
public class UnMuteEvent : ICancellableEvent
{
    internal UnMuteEvent(Player player, bool intercom)
    {
        Player = player;
        Intercom = intercom;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Unmute;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public bool Intercom { get; set; }
}

[PublicAPI]
public class ChangeGroupEvent : ICancellableEvent
{
    internal ChangeGroupEvent(Player player, UserGroup group)
    {
        Player = player;
        Group = group;
    }

    public Player Player { get; }
    public UserGroup Group { get; set; }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.ChangeGroup;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;
}