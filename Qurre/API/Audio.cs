using System;
using CentralAuth;
using JetBrains.Annotations;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using Qurre.API.Addons.Audio;
using Qurre.API.Addons.Audio.Objects;
using Qurre.API.Addons.Audio.Objects.Mirror;
using UnityEngine;
using VoiceChat;
using Object = UnityEngine.Object;

namespace Qurre.API;

using Object = Object;

/// <summary>
///     A layer for simplified work with audio.
/// </summary>
[PublicAPI]
public static class Audio
{
    internal static AudioPlayerBot? LocalHostAudioPlayer;

    /// <summary>
    ///     Audio player that plays on behalf of the host.
    /// </summary>
    public static AudioPlayerBot HostBaseAudioPlayer =>
        LocalHostAudioPlayer ??= new AudioPlayerBot(ReferenceHub._hostHub);

    /// <summary>
    ///     Play audio to a global channel.
    /// </summary>
    /// <param name="audioPlayerBot">Audio player for playback</param>
    /// <param name="nickname">Audio player nickname</param>
    /// <param name="audio">Audio to play</param>
    /// <param name="addDecibels">The number of decibels added to the volume of the playback</param>
    /// <param name="isMute">Mute the audio task?</param>
    /// <param name="isPause">Pause the audio task?</param>
    /// <param name="isLoop">Loop the audio task?</param>
    /// <returns>New instance of <see cref="AudioTask" />.</returns>
    /// <exception cref="System.ArgumentNullException" />
    public static AudioTask PlayAudioInGlobalChannel(
        AudioPlayerBot audioPlayerBot,
        string nickname,
        IAudio audio,
        float addDecibels = 0.0F,
        bool isMute = false,
        bool isPause = false,
        bool isLoop = false
    )
    {
        if (audioPlayerBot is null)
            throw new ArgumentNullException(nameof(audioPlayerBot));
        if (audio is null)
            throw new ArgumentNullException(nameof(audio));

        audioPlayerBot.ReferenceHub.nicknameSync.Network_myNickSync = nickname;
        return audioPlayerBot.Play(audio, VoiceChatChannel.Intercom, addDecibels, isMute, isPause, isLoop);
    }

    /// <summary>
    ///     Play audio into a global channel on behalf of the host.
    /// </summary>
    /// <param name="nickname">Host nickname.</param>
    /// <param name="audio">Audio to play</param>
    /// <param name="addDecibels">The number of decibels added to the volume of the playback</param>
    /// <param name="isMute">Mute the audio task?</param>
    /// <param name="isPause">Pause the audio task?</param>
    /// <param name="isLoop">Loop the audio task?</param>
    /// <returns>New instance of <see cref="AudioTask" />.</returns>
    /// <exception cref="System.ArgumentNullException" />
    public static AudioTask PlayAudioInGlobalChannel(
        string nickname,
        IAudio audio,
        float addDecibels = 0.0F,
        bool isMute = false,
        bool isPause = false,
        bool isLoop = false
    )
    {
        if (audio is null)
            throw new ArgumentNullException(nameof(audio));

        return PlayAudioInGlobalChannel(HostBaseAudioPlayer, nickname, audio, addDecibels, isMute, isPause, isLoop);
    }

    /// <summary>
    ///     Create a new bot to play audio in the Proximity channel.
    /// </summary>
    /// <param name="nickname">Bot nickname</param>
    /// <param name="role">Bot role</param>
    /// <param name="position">Bot position</param>
    /// <param name="rotation">Bot rotation</param>
    /// <returns>Audio player played on behalf of a bot.</returns>
    public static AudioPlayerBot CreateNewAudioPlayer(string nickname, RoleTypeId role, Vector3 position,
        Vector3 rotation)
    {
        // Spawn a new bot.
        GameObject? botObject = Object.Instantiate(NetworkManager.singleton.playerPrefab);
        ZeroConnectionToClient zeroConnection = new();
        ReferenceHub? referenceHub = botObject.GetComponent<ReferenceHub>();

        // Setting up the bot.
        NetworkServer.AddPlayerForConnection(zeroConnection, botObject);
        referenceHub.authManager.InstanceMode = ClientInstanceMode.ReadyClient;
        referenceHub.nicknameSync.Network_myNickSync = nickname;
        referenceHub.nicknameSync.Network_displayName = nickname;
        referenceHub.roleManager.ServerSetRole(role, RoleChangeReason.None);

        // Doing additional bot setup.
        Timing.CallDelayed(0.2F, () =>
        {
            referenceHub.characterClassManager.GodMode = true;
            referenceHub.TryOverridePosition(position);
            referenceHub.TryOverrideRotation(rotation);
        });

        // Creating a new player.
        return new AudioPlayerBot(referenceHub);
    }
}