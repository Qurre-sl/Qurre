using JetBrains.Annotations;
using Qurre.API.Entities.Characters;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class PressAltEvent : ICancellableEvent
{
    internal PressAltEvent(Player player, bool isAllowed)
    {
        Player = player;
        IsAllowed = isAllowed;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.PressAlt;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; }

    public Player Player { get; }
}

[PublicAPI]
public class JumpEvent : IBaseEvent
{
    private const uint EventID = PlayerEvents.Jump;

    internal JumpEvent(Player player)
    {
        Player = player;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = EventID;

    public Player Player { get; }
}