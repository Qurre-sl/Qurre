using CustomPlayerEffects;
using JetBrains.Annotations;
using Qurre.API;
using Qurre.API.Entities.Characters;
using Qurre.API.Enums;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class EffectEnabledEvent : ICancellableEvent
{
    internal EffectEnabledEvent(Player player, StatusEffectBase effect)
    {
        Player = player;
        Effect = effect;
        Type = effect.GetEffectType();
    }

    /// <inheritdoc />
    public uint EventId { get; } = EffectEvents.Enabled;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public StatusEffectBase Effect { get; }
    public EffectType Type { get; }
}

[PublicAPI]
public class EffectDisabledEvent : ICancellableEvent
{
    internal EffectDisabledEvent(Player player, StatusEffectBase effect)
    {
        Player = player;
        Effect = effect;
        Type = effect.GetEffectType();
    }
    
    /// <inheritdoc />
    public uint EventId { get; } = EffectEvents.Disabled;

    /// <inheritdoc />
    public bool IsAllowed { get; set; } = true;

    public Player Player { get; }
    public StatusEffectBase Effect { get; }
    public EffectType Type { get; }
}