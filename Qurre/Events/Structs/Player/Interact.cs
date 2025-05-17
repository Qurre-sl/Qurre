using AdminToys;
using Interactables.Interobjects;
using JetBrains.Annotations;
using MapGeneration.Distributors;
using Qurre.API.Entities.AdminToys;
using Qurre.API.Entities.Characters;
using Qurre.API.Entities.Doors;
using Qurre.API.Entities.Environment;
using Qurre.API.Entities.Structures;
using Qurre.API.Enums;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class InteractDoorEvent : ICancellableEvent
{
    internal InteractDoorEvent(Player player, IDoor door, bool isAllowed)
    {
        Player = player;
        Door = door;
        IsAllowed = isAllowed;
    }

    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.InteractDoor;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; }

    public Player Player { get; }
    public IDoor Door { get; }
}

[PublicAPI]
public class InteractGeneratorEvent : ICancellableEvent
{
    internal InteractGeneratorEvent(Player player, IGenerator generator, GeneratorStatus status)
    {
        Player = player;
        Generator = generator;
        Status = status;
    }

    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.InteractGenerator;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IGenerator Generator { get; }
    public GeneratorStatus Status { get; }
}

[PublicAPI]
public class InteractLiftEvent : ICancellableEvent
{
    internal InteractLiftEvent(Player player, ILift lift)
    {
        Player = player;
        Lift = lift;
        IsAllowed = true;
    }

    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.InteractLift;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; }

    public Player Player { get; }
    public ILift Lift { get; }
}

[PublicAPI]
public class InteractLockerEvent : ICancellableEvent
{
    internal InteractLockerEvent(Player player, ILocker locker, LockerChamber? chamber, bool allow)
    {
        Player = player;
        Locker = locker;
        Chamber = chamber;
        IsAllowed = allow;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.InteractLocker;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; }

    public Player Player { get; }
    public ILocker Locker { get; }
    public LockerChamber? Chamber { get; }
    
    public bool CanOpen { get; set; } // TODO: понять что это значит
}

[PublicAPI]
public class InteractScp330Event : ICancellableEvent
{
    internal InteractScp330Event(Player player, Scp330Interobject scp330)
    {
        Player = player;
        Scp330 = scp330;
    }

    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.InteractScp330;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public Scp330Interobject Scp330 { get; }
}

[PublicAPI]
public class InteractShootingTargetEvent : ICancellableEvent
{
    internal InteractShootingTargetEvent(Player player, IShootingTarget shootingTarget,
        ShootingTarget.TargetButton button)
    {
        Player = player;
        ShootingTarget = shootingTarget;
        Button = button;
    }

    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.InteractShootingTarget;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IShootingTarget ShootingTarget { get; }
    public ShootingTarget.TargetButton Button { get; set; }
}

[PublicAPI]
public class InteractWorkStationEvent : ICancellableEvent
{
    internal InteractWorkStationEvent(Player player, IWorkStation station, byte colliderId)
    {
        Player = player;
        Station = station;
        ColliderId = colliderId;
    }

    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.InteractWorkStation;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public IWorkStation Station { get; }
    public byte ColliderId { get; }
}