using CentralAuth;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using Qurre.API.Addons.Audio;
using Qurre.API.Addons.Audio.Objects;
using Qurre.API.Addons.Audio.Objects.Mirror;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Qurre.API
{
    /// <summary>
    /// A layer for simplified work with audio.
    /// </summary>
    public static class Audio
    {
        /// <summary>
        /// Audio player that plays on behalf of the host.
        /// </summary>
        public static AudioPlayer HostAudioPlayer
        {
            get => _hostAudioPlayer ??= new AudioPlayer(ReferenceHub.HostHub);
        }

        private static AudioPlayer _hostAudioPlayer;

        /// <summary>
        /// Play audio to a global channel.
        /// </summary>
        /// <param name="audioPlayer">Audio player for playback</param>
        /// <param name="nickname">Audio player nickname</param>
        /// <param name="audio">Audio to play</param>
        /// <param name="addDecibels">The number of decibels added to the volume of the playback</param>
        /// <param name="isMute">Mute the audio task?</param>
        /// <param name="isPause">Pause the audio task?</param>
        /// <param name="isLoop">Loop the audio task?</param>
        /// <returns>New instance of <see cref="AudioTask"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static AudioTask PlayAudioInGlobalChannel(
            AudioPlayer audioPlayer, 
            string nickname, 
            IAudio audio, 
            float addDecibels = 0.0F, 
            bool isMute = false, 
            bool isPause = false, 
            bool isLoop = false
            )
        {
            if (audioPlayer == null)
            {
                throw new ArgumentNullException(nameof(audioPlayer));
            }
            else if (audio == null)
            {
                throw new ArgumentNullException(nameof(audio));
            }

            audioPlayer.ReferenceHub.nicknameSync.Network_myNickSync = nickname;
            return audioPlayer.Play(audio, VoiceChat.VoiceChatChannel.Intercom, addDecibels, isMute, isPause, isLoop);
        }

        /// <summary>
        /// Play audio into a global channel on behalf of the host.
        /// </summary>
        /// <param name="nickname">Host nickname.</param>
        /// <param name="audio">Audio to play</param>
        /// <param name="addDecibels">The number of decibels added to the volume of the playback</param>
        /// <param name="isMute">Mute the audio task?</param>
        /// <param name="isPause">Pause the audio task?</param>
        /// <param name="isLoop">Loop the audio task?</param>
        /// <returns>New instance of <see cref="AudioTask"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static AudioTask PlayAudioInGlobalChannel(
            string nickname, 
            IAudio audio, 
            float addDecibels = 0.0F, 
            bool isMute = false, 
            bool isPause = false, 
            bool isLoop = false
            )
        {
            if (audio == null)
            {
                throw new ArgumentNullException(nameof(audio));
            }

            return PlayAudioInGlobalChannel(HostAudioPlayer, nickname, audio, addDecibels, isMute, isPause, isLoop); ;
        }

        /// <summary>
        /// Create a new bot to play audio in the Proximity channel.
        /// </summary>
        /// <param name="nickname">Bot nickname</param>
        /// <param name="role">Bot role</param>
        /// <param name="position">Bot position</param>
        /// <param name="rotation">Bot rotation</param>
        /// <returns>Audio player played on behalf of a bot.</returns>
        public static AudioPlayer CreateNewAudioPlayer(string nickname, RoleTypeId role, Vector3 position, Vector3 rotation)
        {
            // We spawn a new bot.
            var botObject = Object.Instantiate(NetworkManager.singleton.playerPrefab);
            var zeroConnection = new ZeroConnectionToClient();
            var referenceHub = botObject.GetComponent<ReferenceHub>();

            // We setting up the bot.
            NetworkServer.AddPlayerForConnection(zeroConnection, botObject);
            referenceHub.authManager.InstanceMode = ClientInstanceMode.ReadyClient;
            referenceHub.nicknameSync.Network_myNickSync = nickname;
            referenceHub.nicknameSync.Network_displayName = nickname;
            referenceHub.roleManager.ServerSetRole(role, RoleChangeReason.None);

            // We are doing additional bot setup.
            Timing.CallDelayed(0.2F, () =>
            {
                referenceHub.characterClassManager.GodMode = true;
                referenceHub.TryOverridePosition(position, rotation);
            });

            // We are creating a new player.
            var audioPlayer = new AudioPlayer(referenceHub);
            return audioPlayer;
        }
    }
}