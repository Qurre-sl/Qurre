namespace Qurre.Events.Structs;

using PlayerRoles.PlayableScps.Scp096;
using Qurre.API;
using Qurre.API.Objects;

public class Scp096SetStateEvent : IBaseEvent
{
    public uint EventId { get; } = ScpEvents.Scp096SetState;

    public Player Player { get; }
    public Scp096State State { get; }
    public bool Allowed { get; set; }

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
            _ => Scp096State.Unknow,
        };
    }
}

public class Scp096AddTargetEvent : IBaseEvent
{
    public uint EventId { get; } = ScpEvents.Scp096AddTarget;

    public Player Scp { get; }
    public Player Target { get; }
    public bool IsLooking { get; }
    public bool Allowed { get; set; }

    internal Scp096AddTargetEvent(ReferenceHub scp, ReferenceHub target, bool isLooking)
    {
        Scp = scp.GetPlayer();
        Target = target.GetPlayer();
        Allowed = true;
        IsLooking = isLooking;
    }
}