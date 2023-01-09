using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Structs;
using Qurre.API.Objects;

namespace Qurre.Events.Structs
{
    public class InteractGeneratorEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.InteractGenerator;

        public Player Player { get; }
        public Generator Generator { get; }
        public GeneratorStatus Status { get; }
        public bool Allowed { get; set; }

        internal InteractGeneratorEvent(Player player, Generator generator, GeneratorStatus status)
        {
            Player = player;
            Generator = generator;
            Status = status;
            Allowed = true;
        }
    }

    public class InteractLiftEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.InteractLift;

        public Player Player { get; }
        public Lift Lift { get; }
        public bool Allowed { get; set; }

        internal InteractLiftEvent(Player player, Lift lift)
        {
            Player = player;
            Lift = lift;
            Allowed = true;
        }
    }

    public class InteractLockerEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.InteractLocker;

        public Player Player { get; }
        public Locker Locker { get; }
        public Chamber Chamber { get; }
        public bool Allowed { get; set; }

        internal InteractLockerEvent(Player player, Locker locker, Chamber chamber, bool allow)
        {
            Player = player;
            Locker = locker;
            Chamber = chamber;
            Allowed = allow;
        }
    }
}