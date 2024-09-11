using JetBrains.Annotations;
using Qurre.API;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class AlphaStartEvent : IBaseEvent
{
    internal AlphaStartEvent(Player? player, bool automatic)
    {
        Player = player ?? Server.Host;
        Automatic = automatic;
        Allowed = true;
    }

    public Player Player { get; }
    public bool Automatic { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = AlphaEvents.Start;
}

[PublicAPI]
public class AlphaStopEvent : IBaseEvent
{
    internal AlphaStopEvent(Player? player)
    {
        Player = player ?? Server.Host;
        Allowed = true;
    }

    public Player Player { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = AlphaEvents.Stop;
}

[PublicAPI]
public class AlphaDetonateEvent : IBaseEvent
{
    internal AlphaDetonateEvent()
    {
    }

    public uint EventId { get; } = AlphaEvents.Detonate;
}

[PublicAPI]
public class UnlockPanelEvent : IBaseEvent
{
    internal UnlockPanelEvent(Player? player)
    {
        Player = player ?? Server.Host;
        Allowed = true;
    }

    public Player Player { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = AlphaEvents.UnlockPanel;
}