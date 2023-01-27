using CustomPlayerEffects;
using Qurre.API;
using Qurre.API.Objects;

namespace Qurre.Events.Structs
{
    public class EffectEnabledEvent : IBaseEvent
    {
        public uint EventId { get; } = EffectEvents.Enabled;

        public Player Player { get; }
        public StatusEffectBase Effect { get; }
        public EffectType Type { get; }
        public bool Allowed { get; set; }

        internal EffectEnabledEvent(Player player, StatusEffectBase effect)
        {
            Player = player;
            Effect = effect;
            Type = effect.GetEffectType();
            Allowed = true;
        }
    }

    public class EffectDisabledEvent : IBaseEvent
    {
        public uint EventId { get; } = EffectEvents.Disabled;

        public Player Player { get; }
        public StatusEffectBase Effect { get; }
        public EffectType Type { get; }
        public bool Allowed { get; set; }

        internal EffectDisabledEvent(Player player, StatusEffectBase effect)
        {
            Player = player;
            Effect = effect;
            Type = effect.GetEffectType();
            Allowed = true;
        }
    }
}