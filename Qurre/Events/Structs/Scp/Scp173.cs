using JetBrains.Annotations;
using Qurre.API;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class Scp173AddObserverEvent : IBaseEvent
{
    internal Scp173AddObserverEvent(Player pl, Player scp)
    {
        Player = pl;
        Scp = scp;
        Allowed = true;
    }

    public Player Player { get; }
    public Player Scp { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = ScpEvents.Scp173AddObserver;
}

[PublicAPI]
public class Scp173RemovedObserverEvent : IBaseEvent
{
    internal Scp173RemovedObserverEvent(Player pl, Player scp)
    {
        Player = pl;
        Scp = scp;
    }

    public Player Player { get; }
    public Player Scp { get; }
    public uint EventId { get; } = EventID;

    private const uint EventID = ScpEvents.Scp173RemovedObserver;
}

[PublicAPI]
public class Scp173EnableSpeedEvent : IBaseEvent
{
    internal Scp173EnableSpeedEvent(Player pl, bool value)
    {
        Player = pl;
        Value = value;
        Allowed = true;
    }

    public Player Player { get; }
    public bool Value { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = ScpEvents.Scp173EnableSpeed;
}