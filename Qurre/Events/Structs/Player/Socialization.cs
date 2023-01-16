using Qurre.API;

namespace Qurre.Events.Structs
{
    public class CuffEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.Cuff;

        public Player Target { get; }
        public Player Cuffer { get; set; }
        public bool Allowed { get; set; }

        internal CuffEvent(Player target, Player cuffer)
        {
            Target = target;
            Cuffer = cuffer;
            Allowed = true;
        }
    }

    public class UnCuffEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.UnCuff;

        public Player Target { get; }
        public Player Cuffer { get; }
        public bool Allowed { get; set; }

        internal UnCuffEvent(Player target, Player cuffer)
        {
            Target = target;
            Cuffer = cuffer;
            Allowed = true;
        }
    }
}