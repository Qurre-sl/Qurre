using Interactables.Interobjects;
using JetBrains.Annotations;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Structs;
using Qurre.API.Objects;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class InteractDoorEvent : IBaseEvent
{
    internal InteractDoorEvent(Player player, Door door, bool allowed)
    {
        Player = player;
        Door = door;
        Allowed = allowed;
    }

    public Player Player { get; }
    public Door Door { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = PlayerEvents.InteractDoor;
}

[PublicAPI]
public class InteractGeneratorEvent : IBaseEvent
{
    internal InteractGeneratorEvent(Player player, Generator generator, GeneratorStatus status)
    {
        Player = player;
        Generator = generator;
        Status = status;
        Allowed = true;
    }

    public Player Player { get; }
    public Generator Generator { get; }
    public GeneratorStatus Status { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = PlayerEvents.InteractGenerator;
}

[PublicAPI]
public class InteractLiftEvent : IBaseEvent
{
    internal InteractLiftEvent(Player player, Lift lift)
    {
        Player = player;
        Lift = lift;
        Allowed = true;
    }

    public Player Player { get; }
    public Lift Lift { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = PlayerEvents.InteractLift;
}

[PublicAPI]
public class InteractLockerEvent : IBaseEvent
{
    internal InteractLockerEvent(Player player, Locker locker, Chamber? chamber, bool allow)
    {
        Player = player;
        Locker = locker;
        Chamber = chamber;
        Allowed = allow;
    }

    public Player Player { get; }
    public Locker Locker { get; }
    public Chamber? Chamber { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = PlayerEvents.InteractLocker;
}

[PublicAPI]
public class InteractScp330Event : IBaseEvent
{
    internal InteractScp330Event(Player player, Scp330Interobject scp330)
    {
        Player = player;
        Scp330 = scp330;
        Allowed = true;
    }

    public Player Player { get; }
    public Scp330Interobject Scp330 { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = PlayerEvents.InteractScp330;
}

[PublicAPI]
public class InteractShootingTargetEvent : IBaseEvent
{
    internal InteractShootingTargetEvent(Player player, ShootingTarget shootingTarget,
        AdminToys.ShootingTarget.TargetButton button)
    {
        Player = player;
        ShootingTarget = shootingTarget;
        Button = button;
        Allowed = true;
    }

    public Player Player { get; }
    public ShootingTarget ShootingTarget { get; }
    public AdminToys.ShootingTarget.TargetButton Button { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = PlayerEvents.InteractShootingTarget;
}

[PublicAPI]
public class InteractWorkStationEvent : IBaseEvent
{
    internal InteractWorkStationEvent(Player player, WorkStation station, byte colliderId)
    {
        Player = player;
        Station = station;
        ColliderId = colliderId;
        Allowed = true;
    }

    public Player Player { get; }
    public WorkStation Station { get; }
    public byte ColliderId { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = PlayerEvents.InteractWorkStation;
}