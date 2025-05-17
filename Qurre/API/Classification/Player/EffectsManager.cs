using System;
using CustomPlayerEffects;
using CustomRendering;
using JetBrains.Annotations;
using Qurre.API.Objects;

namespace Qurre.API.Classification.Player;

[PublicAPI]
public sealed class EffectsManager
{
    private readonly Controllers.Player _player;

    internal EffectsManager(Controllers.Player pl)
    {
        _player = pl;
    }

    public PlayerEffectsController Controller => _player.ReferenceHub.playerEffectsController;

    public T Get<T>() where T : StatusEffectBase
    {
        return Controller.GetEffect<T>();
    }

    #region Other functions

    public void SetFogType(FogType type)
    {
        Enable<FogControl>();
        SetIntensity<FogControl>((byte)(type + 1));
    }

    #endregion

    #region Get

    public StatusEffectBase? Get(EffectType effect)
    {
        Type type = effect.Type();
        Controller._effectsByType.TryGetValue(type, out StatusEffectBase @base);
        return @base;
    }

    public bool TryGet(EffectType effect, out StatusEffectBase statusEffect)
    {
        Type type = effect.Type();

        if (Controller._effectsByType.TryGetValue(type, out StatusEffectBase @base))
        {
            statusEffect = @base;
            return true;
        }

        statusEffect = null!;
        return false;
    }

    #endregion

    #region Disable

    public void DisableAll()
    {
        Controller.DisableAllEffects();
    }

    public void Disable<T>() where T : StatusEffectBase
    {
        Controller.DisableEffect<T>();
    }

    public void Disable(EffectType effect)
    {
        if (TryGet(effect, out StatusEffectBase @base))
            @base.IsEnabled = false;
    }

    #endregion

    #region Enable

    public void Enable<T>(float duration = 0f, bool addDurationIfActive = false) where T : StatusEffectBase
    {
        Controller.EnableEffect<T>(duration, addDurationIfActive);
    }

    public void Enable(StatusEffectBase effect, float duration = 0f, bool addDurationIfActive = false)
    {
        effect.ServerSetState(1, duration, addDurationIfActive);
    }

    public void Enable(EffectType effect, float duration = 0f, bool addDurationIfActive = false)
    {
        if (TryGet(effect, out StatusEffectBase @base))
            @base.ServerSetState(1, duration, addDurationIfActive);
    }

    #endregion

    #region Extensions

    public bool CheckActive<T>() where T : StatusEffectBase
    {
        return Controller.TryGetEffect(out T @base) && @base.IsEnabled;
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
        if (!TryGet(effect, out StatusEffectBase @base))
            return false;

        @base.Intensity = intensity;
        return true;
    }

    #endregion
}