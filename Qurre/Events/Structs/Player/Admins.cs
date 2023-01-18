using System;
using Qurre.API;

namespace Qurre.Events.Structs
{
    public class BanEvent : IBaseEvent
    {
        internal BanEvent(Player player, Player issuer, DateTime expires, string reason)
        {
            Player = player;
            Issuer = issuer;
            Expires = expires;
            Reason = reason;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.Ban;

        public Player Player { get; }
        public Player Issuer { get; }
        public DateTime Expires { get; set; }
        public string Reason { get; set; }
        public bool Allowed { get; set; }
    }

    public class BannedEvent : IBaseEvent
    {
        internal BannedEvent(Player player, BanDetails details, BanHandler.BanType type)
        {
            Player = player;
            Details = details;
            Type = type;
        }

        public uint EventId { get; } = PlayerEvents.Banned;

        public Player Player { get; }
        public BanDetails Details { get; }
        public BanHandler.BanType Type { get; }
    }

    public class KickEvent : IBaseEvent
    {
        internal KickEvent(Player player, Player issuer, string reason)
        {
            Player = player;
            Issuer = issuer;
            Reason = reason;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.Kick;

        public Player Player { get; }
        public Player Issuer { get; }
        public string Reason { get; set; }
        public bool Allowed { get; set; }
    }

    public class MuteEvent : IBaseEvent
    {
        internal MuteEvent(Player player, bool icom)
        {
            Player = player;
            Intercom = icom;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.Mute;

        public Player Player { get; }
        public bool Intercom { get; set; }
        public bool Allowed { get; set; }
    }

    public class UnMuteEvent : IBaseEvent
    {
        internal UnMuteEvent(Player player, bool icom)
        {
            Player = player;
            Intercom = icom;
            Allowed = true;
        }

        public uint EventId { get; } = PlayerEvents.Unmute;

        public Player Player { get; }
        public bool Intercom { get; set; }
        public bool Allowed { get; set; }
    }
}