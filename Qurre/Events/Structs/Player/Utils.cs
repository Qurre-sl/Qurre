using JetBrains.Annotations;
using Qurre.API.Controllers;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class PressAltEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.PressAlt;

    internal PressAltEvent(Player player, bool allowed)
    {
        Player = player;
        Allowed = allowed;
    }

    public Player Player { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class JumpEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Jump;

    internal JumpEvent(Player player)
    {
        Player = player;
    }

    public Player Player { get; }
    public uint EventId { get; } = EventID;
}