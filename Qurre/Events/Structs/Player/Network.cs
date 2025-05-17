using System;
using System.Net;
using JetBrains.Annotations;
using LiteNetLib;
using Qurre.API;
using Qurre.API.Entities.Characters;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class PreauthEvent : ICancellableEvent
{
    internal PreauthEvent(string userid, IPAddress ip, CentralAuthPreauthFlags flags, string region,
        ConnectionRequest req)
    {
        UserId = userid;
        Ip = ip;
        Flags = flags;
        Region = region;
        Request = req;

        RejectionReason = RejectionReason.NotSpecified;
        RejectionCustomReason = string.Empty;
        RejectionExpiration = DateTime.UtcNow.Ticks;
        RejectionRedirectPort = Server.Port;
        RejectionDelay = 10;
    }

    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Preauth;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public string UserId { get; }
    public IPAddress Ip { get; }
    public CentralAuthPreauthFlags Flags { get; }
    public string Region { get; }
    public ConnectionRequest Request { get; }
    
    public RejectionReason RejectionReason { get; set; }
    public string RejectionCustomReason { get; set; }
    public long RejectionExpiration { get; set; }
    public ushort RejectionRedirectPort { get; set; }
    public byte RejectionDelay { get; set; }
}

[PublicAPI]
public class JoinEvent : IBaseEvent
{
    internal JoinEvent(Player player)
    {
        Player = player;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Join;

    public Player Player { get; }
}

[PublicAPI]
public class LeaveEvent : IBaseEvent
{
    internal LeaveEvent(Player player)
    {
        Player = player;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Leave;

    public Player Player { get; }
}

[PublicAPI]
public class CheckReserveSlotEvent : IBaseEvent
{
    internal CheckReserveSlotEvent(string userid, bool hasReserveSlot = true)
    {
        UserId = userid;
        HasReserveSlot = hasReserveSlot;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.CheckReserveSlot;
    
    public string UserId { get; }
    
    public bool HasReserveSlot { get; set; }
}

[PublicAPI]
public class CheckWhiteListEvent : IBaseEvent
{
    internal CheckWhiteListEvent(string userid, bool isWhitelisted = true)
    {
        UserId = userid;
        IsWhitelisted = isWhitelisted;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.CheckWhiteList;

    public string UserId { get; }
    
    public bool IsWhitelisted { get; set; }
}