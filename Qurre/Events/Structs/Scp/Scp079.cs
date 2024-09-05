using PlayerRoles.PlayableScps.Scp079;
using Qurre.API;
using Qurre.API.Controllers;

namespace Qurre.Events.Structs
{
    public class ActivateGeneratorEvent : IBaseEvent
    {
        public uint EventId { get; } = ScpEvents.ActivateGenerator;

        public Generator Generator { get; }
        public bool Allowed { get; set; }

        internal ActivateGeneratorEvent(Generator generator)
        {
            Generator = generator;
            Allowed = true;
        }
    }

    public class Scp079GetExpEvent : IBaseEvent
    {
        public uint EventId { get; } = ScpEvents.Scp079GetExp;

        public Player Player { get; }
        public Scp079HudTranslation Type { get; }
        public int Amount { get; set; }
        public bool Allowed { get; set; }

        internal Scp079GetExpEvent(Player player, Scp079HudTranslation type, int amount)
        {
            Player = player;
            Type = type;
            Amount = amount;
            Allowed = true;
        }
    }

    public class Scp079NewLvlEvent : IBaseEvent
    {
        public uint EventId { get; } = ScpEvents.Scp079NewLvl;

        public Player Player { get; }
        public int Level { get; set; }
        public bool Allowed { get; set; }

        internal Scp079NewLvlEvent(Player player, int level)
        {
            Player = player;
            Level = level;
            Allowed = true;
        }
    }

    public class Scp079RecontainEvent : IBaseEvent
    {
        public uint EventId { get; } = ScpEvents.Scp079Recontain;

        internal Scp079RecontainEvent() { }
    }

    public class GeneratorStatusEvent : IBaseEvent
    {
        public uint EventId { get; } = ScpEvents.GeneratorStatus;

        public int EnragedCount { get; }
        public int TotalCount { get; }

        internal GeneratorStatusEvent(int enragedCount, int totalCount)
        {
            EnragedCount = enragedCount;
            TotalCount = totalCount;
        }
    }
}