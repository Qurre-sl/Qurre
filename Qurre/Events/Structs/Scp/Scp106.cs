using Qurre.API;

namespace Qurre.Events.Structs
{
    public class Scp106AttackEvent : IBaseEvent
    {
        public uint EventId { get; } = ScpEvents.Scp106Attack;

        public Player Attacker { get; }
        public Player Target { get; }
        public bool Allowed { get; set; }

        internal Scp106AttackEvent(Player attacker, Player target)
        {
            Attacker = attacker;
            Target = target;
            Allowed = true;
        }
    }
}