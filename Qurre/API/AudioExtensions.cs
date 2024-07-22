namespace Qurre.API;

using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp939;
using PlayerRoles.PlayableScps.Scp939.Mimicry;
using PlayerRoles.Subroutines;
using Qurre.API.Addons.Audio;
using Qurre.API.Addons.Audio.Objects;
using RelativePositioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

static public class AudioExtensions
{
    #region Mimic Point
    private const BindingFlags MimicPointPropertiesBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

    static public AudioPlayer PlayFromAll(
        string file,
        string botName = "Dummy",
        List<IAccessConditions> whitelist = null,
        List<IAccessConditions> blacklist = null
        ) => PlayFromAll(
            new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read),
            botName, whitelist, blacklist);

    static public AudioPlayer PlayFromAll(
        Stream stream,
        string botName = "Dummy",
        List<IAccessConditions> whitelist = null,
        List<IAccessConditions> blacklist = null
        )
    {
        // new FileStream("/root/.../...", FileMode.Open, FileAccess.Read, FileShare.Read)
        var streamAudio = new StreamAudio(stream);

        // Create and run player
        var audioPlayer = Audio.CreateNewAudioPlayer(botName, RoleTypeId.Scp939, Vector3.zero, Vector3.zero);
        audioPlayer.RunCoroutine();
        var audioTask = audioPlayer.Play(streamAudio, VoiceChat.VoiceChatChannel.Mimicry);

        // Add white and black lists
        if (whitelist?.Count > 0)
        {
            audioTask.Whitelist.AccessConditions.AddRange(whitelist);
        }
        if (blacklist?.Count > 0)
        {
            audioTask.Blacklist.AccessConditions.AddRange(blacklist);
        }

        Timing.CallDelayed(0.3f, () =>
        {
            try
            {
                var scp939Role = audioPlayer.ReferenceHub.GetComponent<PlayerRoleManager>().CurrentRole as Scp939Role;
                scp939Role.SubroutineModule.TryGetSubroutine<MimicPointController>(out var mimicPoint);

                DoMimicPointInit(mimicPoint);
                Timing.RunCoroutine(DoMimicPointForcePositionJob(audioTask, mimicPoint, audioPlayer));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        });

        return audioPlayer;

        static IEnumerator<float> DoMimicPointForcePositionJob(
            AudioTask audioTask,
            MimicPointController mimicPoint,
            AudioPlayer audioPlayer
            )
        {
            var type = typeof(MimicPointController);

            while (audioTask.IsRunning)
            {
                foreach (var pl in Player.List)
                {
                    var connection = pl.ConnectionToClient;
                    if (!connection.isReady)
                    {
                        continue;
                    }

                    type.GetField(nameof(MimicPointController._syncPos), MimicPointPropertiesBindingFlags)
                        .SetValue(mimicPoint, new RelativePosition(pl.CameraTransform.position));

                    var message = new SubroutineMessage(mimicPoint, true);
                    using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
                    NetworkMessages.Pack(message, networkWriterPooled);

                    var segment = networkWriterPooled.ToArraySegment();
                    connection.Send(segment);
                }

                yield return Timing.WaitForOneFrame;
            }

            audioPlayer.DestroyPlayer();
        }
    }


    static public AudioPlayer PlayFromPlayer(
        string file,
        Player source,
        string botName = "Dummy",
        List<IAccessConditions> whitelist = null,
        List<IAccessConditions> blacklist = null
        ) => PlayFromPlayer(
            new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read),
            source, botName, whitelist, blacklist);

    static public AudioPlayer PlayFromPlayer(
        Stream stream,
        Player source,
        string botName = "Dummy",
        List<IAccessConditions> whitelist = null,
        List<IAccessConditions> blacklist = null
        )
    {
        // new FileStream("/root/.../...", FileMode.Open, FileAccess.Read, FileShare.Read)
        var streamAudio = new StreamAudio(stream);

        // Create and run player
        var audioPlayer = Audio.CreateNewAudioPlayer(botName, RoleTypeId.Scp939, Vector3.zero, Vector3.zero);
        audioPlayer.RunCoroutine();
        var audioTask = audioPlayer.Play(streamAudio, VoiceChat.VoiceChatChannel.Mimicry);

        // Add white and black lists
        if (whitelist?.Count > 0)
        {
            audioTask.Whitelist.AccessConditions.AddRange(whitelist);
        }
        if (blacklist?.Count > 0)
        {
            audioTask.Blacklist.AccessConditions.AddRange(blacklist);
        }

        Timing.CallDelayed(0.5f, () =>
        {
            try
            {
                var scp939Role = audioPlayer.ReferenceHub.GetComponent<PlayerRoleManager>().CurrentRole as Scp939Role;
                scp939Role.SubroutineModule.TryGetSubroutine<MimicPointController>(out var mimicPoint);

                DoMimicPointInit(mimicPoint);
                Timing.RunCoroutine(DoMimicPointPlayerPositionJob(source, audioTask, mimicPoint, audioPlayer));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        });

        return audioPlayer;

        static IEnumerator<float> DoMimicPointPlayerPositionJob(
            Player source,
            AudioTask audioTask,
            MimicPointController mimicPoint,
            AudioPlayer audioPlayer
            )
        {
            var type = typeof(MimicPointController);

            while (audioTask.IsRunning && source is not null && !source.Disconnected)
            {
                type.GetField(nameof(MimicPointController._syncPos), MimicPointPropertiesBindingFlags)
                    .SetValue(mimicPoint, new RelativePosition(source.CameraTransform.position));

                NetworkServer.SendToReady(new SubroutineMessage(mimicPoint, true));
                yield return Timing.WaitForOneFrame;
            }

            audioPlayer.DestroyPlayer();
        }
    }


    static public AudioPlayer PlayFromPosition(
        string file,
        Vector3 source,
        string botName = "Dummy",
        List<IAccessConditions> whitelist = null,
        List<IAccessConditions> blacklist = null
        ) => PlayFromPosition(
            new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read),
            source, botName, whitelist, blacklist);

    static public AudioPlayer PlayFromPosition(
        Stream stream,
        Vector3 source,
        string botName = "Dummy",
        List<IAccessConditions> whitelist = null,
        List<IAccessConditions> blacklist = null
        )
    {
        // new FileStream("/root/.../...", FileMode.Open, FileAccess.Read, FileShare.Read)
        var streamAudio = new StreamAudio(stream);

        // Create and run player
        var audioPlayer = Audio.CreateNewAudioPlayer(botName, RoleTypeId.Scp939, Vector3.zero, Vector3.zero);
        audioPlayer.RunCoroutine();
        var audioTask = audioPlayer.Play(streamAudio, VoiceChat.VoiceChatChannel.Mimicry);

        // Add white and black lists
        if (whitelist?.Count > 0)
        {
            audioTask.Whitelist.AccessConditions.AddRange(whitelist);
        }
        if (blacklist?.Count > 0)
        {
            audioTask.Blacklist.AccessConditions.AddRange(blacklist);
        }

        Timing.CallDelayed(0.5f, () =>
        {
            try
            {
                var scp939Role = audioPlayer.ReferenceHub.GetComponent<PlayerRoleManager>().CurrentRole as Scp939Role;
                scp939Role.SubroutineModule.TryGetSubroutine<MimicPointController>(out var mimicPoint);

                DoMimicPointInit(mimicPoint);

                var type = typeof(MimicPointController);

                type.GetField(nameof(MimicPointController._syncPos), MimicPointPropertiesBindingFlags)
                    .SetValue(mimicPoint, new RelativePosition(source));

                NetworkServer.SendToReady(new SubroutineMessage(mimicPoint, true));

                Timing.RunCoroutine(CheckPlayingAndDestroy(audioPlayer));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        });

        return audioPlayer;
    }

    static void DoMimicPointInit(MimicPointController mimicPoint)
    {
        var type = typeof(MimicPointController);

        type.GetField(nameof(MimicPointController._syncMessage), MimicPointPropertiesBindingFlags)
            .SetValue(mimicPoint, MimicPointController.RpcStateMsg.PlacedByUser);

        type.GetField(nameof(MimicPointController._syncPos), MimicPointPropertiesBindingFlags)
            .SetValue(mimicPoint, new RelativePosition(Vector3.zero));

        type.GetField(nameof(MimicPointController._active), MimicPointPropertiesBindingFlags)
            .SetValue(mimicPoint, true);
    }
    #endregion

    #region Standart
    static public AudioPlayer PlayInIntercom(
        string file,
        string botName = "Dummy",
        List<IAccessConditions> whitelist = null,
        List<IAccessConditions> blacklist = null
        ) => PlayInIntercom(
            new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read),
            botName, whitelist, blacklist);

    static public AudioPlayer PlayInIntercom(
        Stream stream,
        string botName = "Dummy",
        List<IAccessConditions> whitelist = null,
        List<IAccessConditions> blacklist = null
        )
    {
        // new FileStream("/root/.../...", FileMode.Open, FileAccess.Read, FileShare.Read)
        var streamAudio = new StreamAudio(stream);

        // Create and run player
        var audioPlayer = Audio.CreateNewAudioPlayer(botName, RoleTypeId.Spectator, Vector3.zero, Vector3.zero);
        audioPlayer.RunCoroutine();
        var audioTask = audioPlayer.Play(streamAudio, VoiceChat.VoiceChatChannel.Intercom);

        // Add white and black lists
        if (whitelist?.Count > 0)
        {
            audioTask.Whitelist.AccessConditions.AddRange(whitelist);
        }
        if (blacklist?.Count > 0)
        {
            audioTask.Blacklist.AccessConditions.AddRange(blacklist);
        }

        Timing.RunCoroutine(CheckPlayingAndDestroy(audioPlayer));

        return audioPlayer;

    }
    #endregion

    #region Extensions
    static public IEnumerator<float> CheckPlayingAndDestroy(this AudioPlayer audioPlayer)
    {
        yield return Timing.WaitForSeconds(5f);

        while (audioPlayer.AudioTasks.Any(x => !x.IsDone) || (audioPlayer?.CurrentAudioTask?.IsDone == false))
        {
            yield return Timing.WaitForSeconds(0.1f);
        }

        yield return Timing.WaitForSeconds(2f);

        audioPlayer.DestroyPlayer();
    }

    static public void DestroyPlayer(this AudioPlayer audioPlayer)
    {
        audioPlayer.KillCoroutine();
        NetworkServer.Destroy(audioPlayer.ReferenceHub.gameObject);
    }
    #endregion
}