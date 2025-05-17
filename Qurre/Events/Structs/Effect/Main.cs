using CustomPlayerEffects;
using JetBrains.Annotations;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Objects;

// ReSharper disable once CheckNamespace
namespace Qurre.Events.Structs;

[PublicAPI]
public class EffectEnabledEvent : IBaseEvent
{
    private const uint EventID = EffectEvents.Enabled;

    internal EffectEnabledEvent(Player player, StatusEffectBase effect)
    {
        Player = player;
        Effect = effect;
        Type = effect.GetEffectType();
        Allowed = true;
    }

    public Player Player { get; }
    public StatusEffectBase Effect { get; }
    public EffectType Type { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}

[PublicAPI]
public class EffectDisabledEvent : IBaseEvent
{
    private const uint EventID = EffectEvents.Disabled;

    internal EffectDisabledEvent(Player player, StatusEffectBase effect)
    {
        Player = player;
        Effect = effect;
        Type = effect.GetEffectType();
        Allowed = true;
    }

    public Player Player { get; }
    public StatusEffectBase Effect { get; }
    public EffectType Type { get; }
    public bool Allowed { get; set; }
    public uint EventId { get; } = EventID;
}