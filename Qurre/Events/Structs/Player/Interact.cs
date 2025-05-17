using JetBrains.Annotations;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Structs;
using Qurre.API.Objects;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class InteractDoorEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.InteractDoor;

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
}

[PublicAPI]
public class InteractGeneratorEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.InteractGenerator;

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
}

[PublicAPI]
public class InteractLiftEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.InteractLift;

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
}

[PublicAPI]
public class InteractLockerEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.InteractLocker;

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
}

// [PublicAPI]
// public class InteractScp330Event : IBaseEvent
// {
//     private const uint EventID = PlayerEvents.InteractScp330;
//
//     internal InteractScp330Event(Player player, Scp330Interobject scp330)
//     {
//         Player = player;
//         Scp330 = scp330;
//         Allowed = true;
//     }
//
//     public Player Player { get; }
//     public Scp330Interobject Scp330 { get; }
//     public bool Allowed { get; set; }
//     public uint EventId { get; } = EventID;
// }

[PublicAPI]
public class InteractShootingTargetEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.InteractShootingTarget;

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
}

[PublicAPI]
public class InteractWorkStationEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.InteractWorkStation;

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
}