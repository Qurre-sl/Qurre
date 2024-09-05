namespace Qurre.Events.Structs;

using LiteNetLib;
using Qurre.API;
using System.Net;

public class PreauthEvent : IBaseEvent
{
    public uint EventId { get; } = PlayerEvents.Preauth;

    public string UserId { get; }
    public IPAddress Ip { get; }
    public CentralAuthPreauthFlags Flags { get; }
    public string Region { get; }
    public ConnectionRequest Request { get; }
    public bool Allowed { get; set; }

    public RejectionReason RejectionReason { get; set; }
    public string RejectionCustomReason { get; set; }
    public long RejectionExpiration { get; set; }
    public ushort RejectionRedirectPort { get; set; }
    public byte RejectionDelay { get; set; }

    internal PreauthEvent(string userid, IPAddress ip, CentralAuthPreauthFlags flags, string region, ConnectionRequest req)
    {
        UserId = userid;
        Ip = ip;
        Flags = flags;
        Region = region;
        Request = req;
        Allowed = true;

        RejectionReason = RejectionReason.NotSpecified;
        RejectionCustomReason = string.Empty;
        RejectionExpiration = System.DateTime.UtcNow.Ticks;
        RejectionRedirectPort = Server.Port;
        RejectionDelay = 10;
    }
}

public class JoinEvent : IBaseEvent
{
    public uint EventId { get; } = PlayerEvents.Join;

    public Player Player { get; }

    internal JoinEvent(Player player) => Player = player;
}

public class LeaveEvent : IBaseEvent
{
    public uint EventId { get; } = PlayerEvents.Leave;

    public Player Player { get; }

    internal LeaveEvent(Player player) => Player = player;
}

public class CheckReserveSlotEvent : IBaseEvent
{
    public uint EventId { get; } = PlayerEvents.CheckReserveSlot;

    public string UserId { get; }
    public bool Allowed { get; set; }

    internal CheckReserveSlotEvent(string userid, bool allowed = true)
    {
        UserId = userid;
        Allowed = allowed;
    }
}

public class CheckWhiteListEvent : IBaseEvent
{
    public uint EventId { get; } = PlayerEvents.CheckWhiteList;

    public string UserId { get; }
    public bool Allowed { get; set; }

    internal CheckWhiteListEvent(string userid, bool allowed = true)
    {
        UserId = userid;
        Allowed = allowed;
    }
}