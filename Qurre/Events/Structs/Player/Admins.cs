using Qurre.API;
using System;

namespace Qurre.Events.Structs
{
    public class BanEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.Ban;

        public Player Player { get; }
        public Player Issuer { get; }
        public DateTime Expires { get; set; }
        public string Reason { get; set; }
        public bool Allowed { get; set; }

        internal BanEvent(Player player, Player issuer, DateTime expires, string reason)
        {
            Player = player;
            Issuer = issuer;
            Expires = expires;
            Reason = reason;
            Allowed = true;
        }
    }

    public class BannedEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.Banned;

        public Player Player { get; }
        public BanDetails Details { get; }
        public BanHandler.BanType Type { get; }

        internal BannedEvent(Player player, BanDetails details, BanHandler.BanType type)
        {
            Player = player;
            Details = details;
            Type = type;
        }
    }

    public class KickEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.Kick;

        public Player Player { get; }
        public Player Issuer { get; }
        public string Reason { get; set; }
        public bool Allowed { get; set; }

        internal KickEvent(Player player, Player issuer, string reason)
        {
            Player = player;
            Issuer = issuer;
            Reason = reason;
            Allowed = true;
        }
    }

    public class MuteEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.Mute;

        public Player Player { get; }
        public bool Intercom { get; set; }
        public bool Allowed { get; set; }

        internal MuteEvent(Player player, bool icom)
        {
            Player = player;
            Intercom = icom;
            Allowed = true;
        }
    }

    public class UnMuteEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.Unmute;

        public Player Player { get; }
        public bool Intercom { get; set; }
        public bool Allowed { get; set; }

        internal UnMuteEvent(Player player, bool icom)
        {
            Player = player;
            Intercom = icom;
            Allowed = true;
        }
    }

    public class ChangeGroupEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.ChangeGroup;

        public Player Player { get; }
        public UserGroup Group { get; set; }
        public bool Allowed { get; set; }

        internal ChangeGroupEvent(Player player, UserGroup group)
        {
            Player = player;
            Group = group;
            Allowed = true;
        }
    }
}