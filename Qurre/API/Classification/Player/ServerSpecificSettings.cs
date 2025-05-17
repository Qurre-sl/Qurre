using System;
using JetBrains.Annotations;
using Qurre.API.Addons.ServerSpecificSettings;
using UserSettings.ServerSpecific;

namespace Qurre.API.Classification.Player;

/// <summary>
///     Represents server-specific settings for a player.
///     Allows for dynamic application and synchronization of settings variants.
/// </summary>
[PublicAPI]
public sealed class ServerSpecificSettings
{
    private readonly Controllers.Player _player;

    /// <summary>
    ///     Initializes a new instance of <see cref="ServerSpecificSettings" /> for the specified player.
    /// </summary>
    /// <param name="player">The player this instance is associated with.</param>
    internal ServerSpecificSettings(Controllers.Player player)
    {
        _player = player;
        ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingValueReceived;
    }

    /// <summary>
    ///     Gets the current version of the settings.
    ///     Used for synchronization with the client.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    ///     Gets the currently active settings variant.
    /// </summary>
    public BaseSSSVariant? ActiveVariant { get; private set; }

    public void Update(bool preserveVersion = false)
    {
        if (!preserveVersion) Version++;

        ServerSpecificSettingsSync.SendToPlayer(
            _player.ReferenceHub,
            ActiveVariant?.Serialize(),
            Version
        );
    }

    public void Apply(BaseSSSVariant? variant)
    {
        ActiveVariant?.OnDisabled();
        ActiveVariant = variant;
        Update();
    }

    public void Apply<TPerOwnerVariant>() where TPerOwnerVariant : BaseSSSVariant
    {
        Apply(Activator.CreateInstance(typeof(TPerOwnerVariant), _player) as TPerOwnerVariant);
        ActiveVariant?.OnEnabled();
    }

    public void Reset()
    {
        Apply(null);
    }

    public void ResetVersion()
    {
        Version = 0;
    }

    private void OnSettingValueReceived(ReferenceHub referenceHub, ServerSpecificSettingBase settingBase)
    {
        if (_player.ReferenceHub != referenceHub)
            return;

        ActiveVariant?.OnProcessUserInput(settingBase);
    }
}