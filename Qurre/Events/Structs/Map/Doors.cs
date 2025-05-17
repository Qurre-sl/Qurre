using Interactables.Interobjects.DoorUtils;
using JetBrains.Annotations;
using Qurre.API.Entities.Doors;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class DamageDoorEvent : ICancellableEvent
{
    internal DamageDoorEvent(IBreakableDoor door, DoorDamageType type, float damage)
    {
        Door = door;
        Type = type;
        Damage = damage;
    }

    public IBreakableDoor Door { get; }
    public DoorDamageType Type { get; }
    public float Damage { get; set; }

    /// <inheritdoc />
    public uint EventId { get; } = MapEvents.DamageDoor;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;
}

[PublicAPI]
public class LockDoorEvent : ICancellableEvent
{
    internal LockDoorEvent(IDoor door, DoorLockReason reason, bool newState)
    {
        Door = door;
        Reason = reason;
        NewState = newState;
    }

    /// <inheritdoc />
    public uint EventId { get; } = MapEvents.LockDoor;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public IDoor Door { get; }
    public DoorLockReason Reason { get; }
    public bool NewState { get; set; }
}

[PublicAPI]
public class OpenDoorEvent : ICancellableEvent
{
    internal OpenDoorEvent(IDoor door, DoorEventOpenerExtension.OpenerEventType type)
    {
        Door = door;
        Type = type;
    }

    /// <inheritdoc />
    public uint EventId { get; } = MapEvents.OpenDoor;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public IDoor Door { get; }
    public DoorEventOpenerExtension.OpenerEventType Type { get; }
}