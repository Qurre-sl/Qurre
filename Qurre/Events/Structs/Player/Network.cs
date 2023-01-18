using System;
using System.Net;
using LiteNetLib;
using Qurre.API;

namespace Qurre.Events.Structs
{
    public class PreauthEvent : IBaseEvent
    {
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
            RejectionExpiration = DateTime.UtcNow.Ticks;
            RejectionRedirectPort = Server.Port;
            RejectionDelay = 10;
        }

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
    }

    public class JoinEvent : IBaseEvent
    {
        internal JoinEvent(Player player) => Player = player;

        public uint EventId { get; } = PlayerEvents.Join;

        public Player Player { get; }
    }

    public class LeaveEvent : IBaseEvent
    {
        internal LeaveEvent(Player player) => Player = player;

        public uint EventId { get; } = PlayerEvents.Leave;

        public Player Player { get; }
    }

    public class CheckReserveSlotEvent : IBaseEvent
    {
        internal CheckReserveSlotEvent(string userid, bool allowed = true)
        {
            UserId = userid;
            Allowed = allowed;
        }

        public uint EventId { get; } = PlayerEvents.CheckReserveSlot;

        public string UserId { get; }
        public bool Allowed { get; set; }
    }
}