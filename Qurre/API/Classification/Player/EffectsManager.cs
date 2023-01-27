namespace Qurre.API.Classification.Player
{
    using CustomPlayerEffects;
    using Qurre.API;
    using Qurre.API.Objects;

    public sealed class EffectsManager
    {
        private readonly Player _player;
        internal EffectsManager(Player pl) => _player = pl;

        public PlayerEffectsController Controller => _player.ReferenceHub.playerEffectsController;

        public T Get<T>() where T : StatusEffectBase
            => Controller.GetEffect<T>();

        #region Get
        public StatusEffectBase Get(EffectType effect)
        {
            var type = effect.Type();
            Controller._effectsByType.TryGetValue(type, out StatusEffectBase @base);
            return @base;
        }
        public bool TryGet(EffectType effect, out StatusEffectBase statusEffect)
        {
            var type = effect.Type();

            if (Controller._effectsByType.TryGetValue(type, out StatusEffectBase @base))
            {
                statusEffect = @base;
                return true;
            }

            statusEffect = null;
            return false;
        }
        #endregion

        #region Disable
        public void DisableAll()
            => Controller.DisableAllEffects();

        public void Disable<T>() where T : StatusEffectBase
            => Controller.DisableEffect<T>();

        public void Disable(EffectType effect)
        {
            if (TryGet(effect, out StatusEffectBase @base))
                @base.IsEnabled = false;
        }
        #endregion

        #region Enable
        public void Enable<T>(float duration = 0f, bool addDurationIfActive = false) where T : StatusEffectBase
            => Controller.EnableEffect<T>(duration, addDurationIfActive);

        public void Enable(StatusEffectBase effect, float duration = 0f, bool addDurationIfActive = false)
            => effect.ServerSetState(1, duration, addDurationIfActive);

        public void Enable(EffectType effect, float duration = 0f, bool addDurationIfActive = false)
        {
            if (TryGet(effect, out StatusEffectBase @base))
                @base.ServerSetState(1, duration, addDurationIfActive);
        }
        #endregion

        #region Extensions
        public bool CheckActive<T>() where T : StatusEffectBase
        {
            if (Controller.TryGetEffect(out T @base))
                return @base.IsEnabled;
            return false;
        }

        public byte GetIntensity<T>() where T : StatusEffectBase
        {
            StatusEffectBase @base = Get<T>();
            return @base.Intensity;
        }

        public void SetIntensity<T>(byte intensity) where T : StatusEffectBase
        {
            StatusEffectBase @base = Get<T>();
            @base.Intensity = intensity;
        }
        public bool TrySetIntensity(EffectType effect, byte intensity)
        {
            if (TryGet(effect, out StatusEffectBase @base))
            {
                @base.Intensity = intensity;
                return true;
            }
            return false;
        }
        #endregion
    }
}