using JetBrains.Annotations;
using Qurre.API.Entities.Characters;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class CuffEvent : ICancellableEvent
{
    internal CuffEvent(Player target, Player cuffer)
    {
        Target = target;
        Cuffer = cuffer;
        IsAllowed = true;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.Cuff;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; }

    public Player Target { get; }
    public Player Cuffer { get; set; }
}

[PublicAPI]
public class UnCuffEvent : ICancellableEvent
{
    private const uint EventID = PlayerEvents.UnCuff;

    internal UnCuffEvent(Player target, Player cuffer)
    {
        Target = target;
        Cuffer = cuffer;
        IsAllowed = true;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = EventID;
    
    /// <inheritdoc />
    public bool IsAllowed { get; set; }

    public Player Target { get; }
    public Player Cuffer { get; }
}

[PublicAPI]
public class ChangeSpectateEvent : IBaseEvent
{
    internal ChangeSpectateEvent(Player player, Player? previousTarget, Player? newTarget)
    {
        Player = player;
        PreviousTarget = previousTarget;
        NewTarget = newTarget;
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = PlayerEvents.ChangeSpectate;

    public Player Player { get; }
    public Player? PreviousTarget { get; }
    public Player? NewTarget { get; }
}