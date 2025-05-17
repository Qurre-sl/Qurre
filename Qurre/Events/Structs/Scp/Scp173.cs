using JetBrains.Annotations;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp173AddObserverEvent : IBaseEvent
{
    private const uint EventID = ScpEvents.Scp173AddObserver;

    internal Scp173AddObserverEvent(Player target, Player scp)
    {
        Target = target;
        Scp = scp;
        Allowed = true;
    }

    public Player Target { get; }
    public Player Scp { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class Scp173RemovedObserverEvent : IBaseEvent
{
    private const uint EventID = ScpEvents.Scp173RemovedObserver;

    internal Scp173RemovedObserverEvent(Player target, Player scp)
    {
        Target = target;
        Scp = scp;
    }

    public Player Target { get; }
    public Player Scp { get; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class Scp173EnableSpeedEvent : IBaseEvent
{
    private const uint EventID = ScpEvents.Scp173EnableSpeed;

    internal Scp173EnableSpeedEvent(Player pl, bool active)
    {
        Player = pl;
        Active = active;
        Allowed = true;
    }

    public Player Player { get; }
    public bool Active { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}