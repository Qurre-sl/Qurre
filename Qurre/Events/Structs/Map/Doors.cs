using Interactables.Interobjects.DoorUtils;
using JetBrains.Annotations;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class DamageDoorEvent : IBaseEvent
{
    internal DamageDoorEvent(Door door, DoorDamageType type, float damage)
    {
        Door = door;
        Type = type;
        Damage = damage;
        Allowed = true;
    }

    public Door Door { get; }
    public DoorDamageType Type { get; }
    public float Damage { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = MapEvents.DamageDoor;
}

[PublicAPI]
public class LockDoorEvent : IBaseEvent
{
    internal LockDoorEvent(Door door, DoorLockReason reason, bool @new)
    {
        Door = door;
        Reason = reason;
        NewState = @new;
        Allowed = true;
    }

    public Door Door { get; }
    public DoorLockReason Reason { get; }
    public bool NewState { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = MapEvents.LockDoor;
}

[PublicAPI]
public class OpenDoorEvent : IBaseEvent
{
    internal OpenDoorEvent(Door door, DoorEventOpenerExtension.OpenerEventType type)
    {
        Door = door;
        Type = type;
        Allowed = true;
    }

    public Door Door { get; }
    public DoorEventOpenerExtension.OpenerEventType Type { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = MapEvents.OpenDoor;
}