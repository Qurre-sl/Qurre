namespace Qurre.Events.Structs;

using Qurre.API;

public class Scp173AddObserverEvent : IBaseEvent
{
    public uint EventId { get; } = ScpEvents.Scp173AddObserver;

    public Player Player { get; }
    public Player Scp { get; }
    public bool Allowed { get; set; }

    internal Scp173AddObserverEvent(Player pl, Player scp)
    {
        Player = pl;
        Scp = scp;
        Allowed = true;
    }
}

public class Scp173RemovedObserverEvent : IBaseEvent
{
    public uint EventId { get; } = ScpEvents.Scp173RemovedObserver;

    public Player Player { get; }
    public Player Scp { get; }

    internal Scp173RemovedObserverEvent(Player pl, Player scp)
    {
        Player = pl;
        Scp = scp;
    }
}

public class Scp173EnableSpeedEvent : IBaseEvent
{
    public uint EventId { get; } = ScpEvents.Scp173EnableSpeed;

    public Player Player { get; }
    public bool Value { get; }
    public bool Allowed { get; set; }

    internal Scp173EnableSpeedEvent(Player pl, bool value)
    {
        Player = pl;
        Value = value;
        Allowed = true;
    }
}