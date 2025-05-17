using JetBrains.Annotations;
using Qurre.API;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class AlphaStartEvent : IBaseEvent
{
    private const uint EventID = AlphaEvents.Start;

    internal AlphaStartEvent(Player? player, bool automatic)
    {
        Player = player ?? Server.Host;
        Automatic = automatic;
        Allowed = true;
    }

    public Player Player { get; }
    public bool Automatic { get; set; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class AlphaStopEvent : IBaseEvent
{
    private const uint EventID = AlphaEvents.Stop;

    internal AlphaStopEvent(Player? player)
    {
        Player = player ?? Server.Host;
        Allowed = true;
    }

    public Player Player { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class AlphaDetonateEvent : IBaseEvent
{
    private const uint EventID = AlphaEvents.Detonate;

    internal AlphaDetonateEvent()
    {
    }

    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class UnlockPanelEvent : IBaseEvent
{
    private const uint EventID = AlphaEvents.UnlockPanel;

    internal UnlockPanelEvent(Player? player)
    {
        Player = player ?? Server.Host;
        Allowed = true;
    }

    public Player Player { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}