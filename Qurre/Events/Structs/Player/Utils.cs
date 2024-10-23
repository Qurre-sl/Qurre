using JetBrains.Annotations;
using Qurre.API;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class PressAltEvent : IBaseEvent
{
    internal PressAltEvent(Player player, bool allowed)
    {
        Player = player;
        Allowed = allowed;
    }

    public Player Player { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;

    private const uint EventID = PlayerEvents.PressAlt;
}

[PublicAPI]
public class JumpEvent : IBaseEvent
{
    internal JumpEvent(Player player)
    {
        Player = player;
    }

    public Player Player { get; }
    public uint EventId { get; } = EventID;

    private const uint EventID = PlayerEvents.Jump;
}