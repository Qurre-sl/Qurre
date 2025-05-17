using JetBrains.Annotations;
using LiteNetLib;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class PreauthEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Preauth;

    internal PreauthEvent(string userid, string ip, CentralAuthPreauthFlags flags, string region,
        ConnectionRequest req)
    {
        UserId = userid;
        Ip = ip;
        Flags = flags;
        Region = region;
        Request = req;
        Allowed = true;
    }

    public string UserId { get; }
    public string Ip { get; }
    public CentralAuthPreauthFlags Flags { get; }
    public string Region { get; }
    public ConnectionRequest Request { get; }
    public bool Allowed { get; set; }

    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class JoinEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Join;

    internal JoinEvent(Player player)
    {
        Player = player;
    }

    public Player Player { get; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class LeaveEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Leave;

    internal LeaveEvent(Player player)
    {
        Player = player;
    }

    public Player Player { get; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class CheckReserveSlotEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.CheckReserveSlot;

    internal CheckReserveSlotEvent(string userid, bool allowed = true)
    {
        UserId = userid;
        Allowed = allowed;
    }

    public string UserId { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class CheckWhiteListEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.CheckWhiteList;

    internal CheckWhiteListEvent(string userid, bool allowed = true)
    {
        UserId = userid;
        Allowed = allowed;
    }

    public string UserId { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}