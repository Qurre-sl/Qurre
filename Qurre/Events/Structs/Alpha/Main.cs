using Qurre.API;

namespace Qurre.Events.Structs
{
    public class AlphaStartEvent : IBaseEvent
    {
        public uint EventId { get; } = AlphaEvents.Start;

        public Player Player { get; }
        public bool Automatic { get; set; }
        public bool Allowed { get; set; }

        internal AlphaStartEvent(Player player, bool automatic)
        {
            Player = player ?? Server.Host;
            Automatic = automatic;
            Allowed = true;
        }
    }

    public class AlphaStopEvent : IBaseEvent
    {
        public uint EventId { get; } = AlphaEvents.Stop;

        public Player Player { get; }
        public bool Allowed { get; set; }

        internal AlphaStopEvent(Player player)
        {
            Player = player ?? Server.Host;
            Allowed = true;
        }
    }

    public class AlphaDetonateEvent : IBaseEvent
    {
        public uint EventId { get; } = AlphaEvents.Detonate;

        internal AlphaDetonateEvent() { }
    }

    public class UnlockPanelEvent : IBaseEvent
    {
        public uint EventId { get; } = AlphaEvents.UnlockPanel;

        public Player Player { get; }
        public bool Allowed { get; set; }

        internal UnlockPanelEvent(Player player)
        {
            Player = player ?? Server.Host;
            Allowed = true;
        }
    }
}