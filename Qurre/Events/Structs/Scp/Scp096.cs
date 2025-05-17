using JetBrains.Annotations;
using PlayerRoles.PlayableScps.Scp096;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Objects;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp096SetStateEvent : IBaseEvent
{
    private const uint EventID = ScpEvents.Scp096SetState;

    internal Scp096SetStateEvent(Player pl, Scp096State state)
    {
        Player = pl;
        State = state;
        Allowed = true;
    }

    internal Scp096SetStateEvent(Player pl, Scp096RageState state)
    {
        Player = pl;
        Allowed = true;

        State = state switch
        {
            Scp096RageState.Calming => Scp096State.Calming,
            Scp096RageState.Enraged => Scp096State.Enraged,
            Scp096RageState.Distressed => Scp096State.Distressed,
            Scp096RageState.Docile => Scp096State.Docile,
            _ => Scp096State.Unknown
        };
    }

    public Player Player { get; }
    public Scp096State State { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class Scp096AddTargetEvent : IBaseEvent
{
    private const uint EventID = ScpEvents.Scp096AddTarget;

    internal Scp096AddTargetEvent(ReferenceHub scp, ReferenceHub target, bool isLooking)
    {
        Scp = scp.GetPlayer() ?? Server.Host;
        Target = target.GetPlayer() ?? Server.Host;
        Allowed = true;
        IsLooking = isLooking;
    }

    public Player Scp { get; }
    public Player Target { get; }
    public bool IsLooking { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}