using LiteNetLib;
using Qurre.API;
using System.Net;

namespace Qurre.Events.Structs
{
    public class PreauthEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.Preauth;

        public string UserId { get; }
        public IPAddress Ip { get; }
        public CentralAuthPreauthFlags Flags { get; }
        public string Region { get; }
        public ConnectionRequest Request { get; }
        public bool Allowed { get; set; }


        internal PreauthEvent(string userid, IPAddress ip, CentralAuthPreauthFlags flags, string region, ConnectionRequest req)
        {
            UserId = userid;
            Ip = ip;
            Flags = flags;
            Region = region;
            Request = req;
            Allowed = true;
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
}