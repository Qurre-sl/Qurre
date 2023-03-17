using Interactables.Interobjects.DoorUtils;
using Qurre.API.Controllers;

namespace Qurre.Events.Structs
{
    public class DamageDoorEvent : IBaseEvent
    {
        public uint EventId { get; } = MapEvents.DamageDoor;

        public Door Door { get; }
        public DoorDamageType Type { get; }
        public float Damage { get; set; }
        public bool Allowed { get; set; }

        internal DamageDoorEvent(Door door, DoorDamageType type, float damage)
        {
            Door = door;
            Type = type;
            Damage = damage;
            Allowed = true;
        }
    }

    public class LockDoorEvent : IBaseEvent
    {
        public uint EventId { get; } = MapEvents.LockDoor;

        public Door Door { get; }
        public DoorLockReason Reason { get; }
        public bool NewState { get; set; }
        public bool Allowed { get; set; }

        internal LockDoorEvent(Door door, DoorLockReason reason, bool @new)
        {
            Door = door;
            Reason = reason;
            NewState = @new;
            Allowed = true;
        }
    }

    public class OpenDoorEvent : IBaseEvent
    {
        public uint EventId { get; } = MapEvents.OpenDoor;

        public Door Door { get; }
        public DoorEventOpenerExtension.OpenerEventType Type { get; }
        public bool Allowed { get; set; }

        internal OpenDoorEvent(Door door, DoorEventOpenerExtension.OpenerEventType type)
        {
            Door = door;
            Type = type;
            Allowed = true;
        }
    }
}