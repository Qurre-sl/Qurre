using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp939;
using PlayerRoles.PlayableScps.Scp939.Mimicry;
using PlayerRoles.Subroutines;
using Qurre.API.Addons.Audio;
using Qurre.API.Addons.Audio.Objects;
using Qurre.API.Controllers;
using RelativePositioning;
using UnityEngine;
using VoiceChat;

namespace Qurre.API;

[PublicAPI]
public static class AudioExtensions
{
    #region Extensions

    public static IEnumerator<float> CheckPlayingAndDestroy(this BaseAudioPlayer baseAudioPlayer)
    {
        yield return Timing.WaitForSeconds(5f);

        while (baseAudioPlayer.AudioTasks.Any(x => !x.IsDone) || baseAudioPlayer.CurrentAudioTask?.IsDone == false)
            yield return Timing.WaitForSeconds(0.1f);

        yield return Timing.WaitForSeconds(2f);

        baseAudioPlayer.DestroySelf();
    }

    #endregion

    #region Mimic Point

    private const BindingFlags MimicPointPropertiesBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

    public static AudioPlayerBot PlayFromAll(
        string file,
        string botName = "Dummy",
        List<IAccessConditions>? whitelist = null,
        List<IAccessConditions>? blacklist = null
    )
    {
        return PlayFromAll(
            new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read),
            botName, whitelist, blacklist);
    }

    public static AudioPlayerBot PlayFromAll(
        Stream stream,
        string botName = "Dummy",
        List<IAccessConditions>? whitelist = null,
        List<IAccessConditions>? blacklist = null
    )
    {
        // new FileStream("/root/.../...", FileMode.Open, FileAccess.Read, FileShare.Read)
        StreamAudio streamAudio = new(stream);

        // Create and run player
        AudioPlayerBot audioPlayer = Audio.CreateNewAudioPlayer(botName, RoleTypeId.Scp939, Vector3.zero, Vector3.zero);
        audioPlayer.RunCoroutine();
        AudioTask audioTask = audioPlayer.Play(streamAudio, VoiceChatChannel.Mimicry);

        // Add whitelist and blacklist
        if (whitelist?.Count > 0) audioTask.Whitelist.AccessConditions.AddRange(whitelist);
        if (blacklist?.Count > 0) audioTask.Blacklist.AccessConditions.AddRange(blacklist);

        Timing.CallDelayed(0.3f, () =>
        {
            try
            {
                if (audioPlayer.ReferenceHub.GetComponent<PlayerRoleManager>().CurrentRole is not Scp939Role scp939Role)
                    return;

                scp939Role.SubroutineModule.TryGetSubroutine<MimicPointController>(
                    out MimicPointController? mimicPoint);

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
            BaseAudioPlayer audioPlayer
        )
        {
            Type type = typeof(MimicPointController);

            while (audioTask.IsRunning)
            {
                foreach (Player? pl in Player.List)
                {
                    NetworkConnectionToClient connection = pl.ConnectionToClient;
                    if (!connection.isReady) continue;

                    type.GetField(nameof(MimicPointController._syncPos), MimicPointPropertiesBindingFlags)
                        ?.SetValue(mimicPoint, new RelativePosition(pl.CameraTransform.position));

                    SubroutineMessage message = new(mimicPoint, true);
                    using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
                    NetworkMessages.Pack(message, networkWriterPooled);

                    var segment = networkWriterPooled.ToArraySegment();
                    connection.Send(segment);
                }

                yield return Timing.WaitForOneFrame;
            }

            audioPlayer.DestroySelf();
        }
    }


    public static AudioPlayerBot PlayFromPlayer(
        string file,
        Player source,
        string botName = "Dummy",
        List<IAccessConditions>? whitelist = null,
        List<IAccessConditions>? blacklist = null
    )
    {
        return PlayFromPlayer(
            new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read),
            source, botName, whitelist, blacklist);
    }

    public static AudioPlayerBot PlayFromPlayer(
        Stream stream,
        Player source,
        string botName = "Dummy",
        List<IAccessConditions>? whitelist = null,
        List<IAccessConditions>? blacklist = null
    )
    {
        // new FileStream("/root/.../...", FileMode.Open, FileAccess.Read, FileShare.Read)
        StreamAudio streamAudio = new(stream);

        // Create and run player
        AudioPlayerBot audioPlayer = Audio.CreateNewAudioPlayer(botName, RoleTypeId.Scp939, Vector3.zero, Vector3.zero);
        audioPlayer.RunCoroutine();
        AudioTask audioTask = audioPlayer.Play(streamAudio, VoiceChatChannel.Mimicry);

        // Add whitelist and blacklist
        if (whitelist?.Count > 0) audioTask.Whitelist.AccessConditions.AddRange(whitelist);
        if (blacklist?.Count > 0) audioTask.Blacklist.AccessConditions.AddRange(blacklist);

        Timing.CallDelayed(0.5f, () =>
        {
            try
            {
                if (audioPlayer.ReferenceHub.GetComponent<PlayerRoleManager>().CurrentRole is not Scp939Role scp939Role)
                    return;

                scp939Role.SubroutineModule.TryGetSubroutine<MimicPointController>(
                    out MimicPointController? mimicPoint);

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
            BaseAudioPlayer audioPlayer
        )
        {
            Type type = typeof(MimicPointController);

            while (audioTask.IsRunning && !source.Disconnected)
            {
                type.GetField(nameof(MimicPointController._syncPos), MimicPointPropertiesBindingFlags)
                    ?.SetValue(mimicPoint, new RelativePosition(source.CameraTransform.position));

                NetworkServer.SendToReady(new SubroutineMessage(mimicPoint, true));
                yield return Timing.WaitForOneFrame;
            }

            audioPlayer.DestroySelf();
        }
    }


    public static AudioPlayerBot PlayFromPosition(
        string file,
        Vector3 source,
        string botName = "Dummy",
        List<IAccessConditions>? whitelist = null,
        List<IAccessConditions>? blacklist = null
    )
    {
        return PlayFromPosition(
            new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read),
            source, botName, whitelist, blacklist);
    }

    public static AudioPlayerBot PlayFromPosition(
        Stream stream,
        Vector3 source,
        string botName = "Dummy",
        List<IAccessConditions>? whitelist = null,
        List<IAccessConditions>? blacklist = null
    )
    {
        // new FileStream("/root/.../...", FileMode.Open, FileAccess.Read, FileShare.Read)
        StreamAudio streamAudio = new(stream);

        // Create and run player
        AudioPlayerBot audioPlayer = Audio.CreateNewAudioPlayer(botName, RoleTypeId.Scp939, Vector3.zero, Vector3.zero);
        audioPlayer.RunCoroutine();
        AudioTask audioTask = audioPlayer.Play(streamAudio, VoiceChatChannel.Mimicry);

        // Add whitelist and blacklist
        if (whitelist?.Count > 0) audioTask.Whitelist.AccessConditions.AddRange(whitelist);
        if (blacklist?.Count > 0) audioTask.Blacklist.AccessConditions.AddRange(blacklist);

        Timing.CallDelayed(0.5f, () =>
        {
            try
            {
                if (audioPlayer.ReferenceHub.GetComponent<PlayerRoleManager>().CurrentRole is not Scp939Role scp939Role)
                    return;

                scp939Role.SubroutineModule.TryGetSubroutine<MimicPointController>(
                    out MimicPointController? mimicPoint);

                DoMimicPointInit(mimicPoint);

                Type type = typeof(MimicPointController);

                type.GetField(nameof(MimicPointController._syncPos), MimicPointPropertiesBindingFlags)
                    ?.SetValue(mimicPoint, new RelativePosition(source));

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

    private static void DoMimicPointInit(MimicPointController mimicPoint)
    {
        Type type = typeof(MimicPointController);

        type.GetField(nameof(MimicPointController._syncMessage), MimicPointPropertiesBindingFlags)
            ?.SetValue(mimicPoint, MimicPointController.RpcStateMsg.PlacedByUser);

        type.GetField(nameof(MimicPointController._syncPos), MimicPointPropertiesBindingFlags)
            ?.SetValue(mimicPoint, new RelativePosition(Vector3.zero));

        type.GetField(nameof(MimicPointController._active), MimicPointPropertiesBindingFlags)
            ?.SetValue(mimicPoint, true);
    }

    #endregion

    #region Standart

    public static AudioPlayerBot PlayInIntercom(
        string file,
        string botName = "Dummy",
        List<IAccessConditions>? whitelist = null,
        List<IAccessConditions>? blacklist = null
    )
    {
        return PlayInIntercom(
            new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read),
            botName, whitelist, blacklist);
    }

    public static AudioPlayerBot PlayInIntercom(
        Stream stream,
        string botName = "Dummy",
        List<IAccessConditions>? whitelist = null,
        List<IAccessConditions>? blacklist = null
    )
    {
        // new FileStream("/root/.../...", FileMode.Open, FileAccess.Read, FileShare.Read)
        StreamAudio streamAudio = new(stream);

        // Create and run player
        AudioPlayerBot audioPlayer =
            Audio.CreateNewAudioPlayer(botName, RoleTypeId.Spectator, Vector3.zero, Vector3.zero);
        audioPlayer.RunCoroutine();
        AudioTask audioTask = audioPlayer.Play(streamAudio, VoiceChatChannel.Intercom);

        // Add whitelist and blacklist
        if (whitelist?.Count > 0) audioTask.Whitelist.AccessConditions.AddRange(whitelist);
        if (blacklist?.Count > 0) audioTask.Blacklist.AccessConditions.AddRange(blacklist);

        Timing.RunCoroutine(CheckPlayingAndDestroy(audioPlayer));

        return audioPlayer;
    }

    #endregion
}