using Interactables.Interobjects;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Structs;
using Qurre.API.Objects;

namespace Qurre.Events.Structs
{
    public class InteractDoorEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.InteractDoor;

        public Player Player { get; }
        public Door Door { get; }
        public bool Allowed { get; set; }

        internal InteractDoorEvent(Player player, Door door, bool allowed)
        {
            Player = player;
            Door = door;
            Allowed = allowed;
        }
    }

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

    public class InteractScp330Event : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.InteractScp330;

        public Player Player { get; }
        public Scp330Interobject Scp330 { get; }
        public bool Allowed { get; set; }

        internal InteractScp330Event(Player player, Scp330Interobject scp330)
        {
            Player = player;
            Scp330 = scp330;
            Allowed = true;
        }
    }

    public class InteractShootingTargetEvent : IBaseEvent
    {
        public uint EventId { get; } = PlayerEvents.InteractShootingTarget;

        public Player Player { get; }
        public ShootingTarget ShootingTarget { get; }
        public AdminToys.ShootingTarget.TargetButton Button { get; set; }
        public bool Allowed { get; set; }

        internal InteractShootingTargetEvent(Player player, ShootingTarget shootingTarget, AdminToys.ShootingTarget.TargetButton button)
        {
            Player = player;
            ShootingTarget = shootingTarget;
            Button = button;
            Allowed = true;
        }
    }
}