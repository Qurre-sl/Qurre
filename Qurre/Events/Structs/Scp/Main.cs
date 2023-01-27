using Qurre.API;
using Qurre.API.Objects;

namespace Qurre.Events.Structs
{
    public class ScpAttackEvent : IBaseEvent
    {
        public uint EventId { get; } = ScpEvents.Attack;

        public Player Attacker { get; }
        public Player Target { get; }
        public ScpAttackType Type { get; }
        public float Damage { get; set; }
        public bool Allowed { get; set; }

        internal ScpAttackEvent(Player attacker, Player target, ScpAttackType type)
        {
            Attacker = attacker;
            Target = target;
            Type = type;
            Allowed = true;
        }
    }
}