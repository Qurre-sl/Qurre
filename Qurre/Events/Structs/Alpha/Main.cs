using Qurre.API;

namespace Qurre.Events.Structs
{
    public class AlphaStartEvent : IBaseEvent
    {
        internal AlphaStartEvent(Player player, bool automatic)
        {
            Player = player ?? Server.Host;
            Automatic = automatic;
            Allowed = true;
        }

        public uint EventId { get; } = AlphaEvents.Start;

        public Player Player { get; }
        public bool Automatic { get; set; }
        public bool Allowed { get; set; }
    }

    public class AlphaStopEvent : IBaseEvent
    {
        internal AlphaStopEvent(Player player)
        {
            Player = player ?? Server.Host;
            Allowed = true;
        }

        public uint EventId { get; } = AlphaEvents.Stop;

        public Player Player { get; }
        public bool Allowed { get; set; }
    }

    public class AlphaDetonateEvent : IBaseEvent
    {
        internal AlphaDetonateEvent() { }

        public uint EventId { get; } = AlphaEvents.Detonate;
    }

    public class UnlockPanelEvent : IBaseEvent
    {
        internal UnlockPanelEvent(Player player)
        {
            Player = player ?? Server.Host;
            Allowed = true;
        }

        public uint EventId { get; } = AlphaEvents.UnlockPanel;

        public Player Player { get; }
        public bool Allowed { get; set; }
    }
}