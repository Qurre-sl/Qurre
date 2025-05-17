using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MEC;
using Qurre.API.Addons.Audio.Objects;
using UnityEngine;
using VoiceChat;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;

namespace Qurre.API.Addons.Audio;

[PublicAPI]
public abstract class BaseAudioPlayer : IEquatable<BaseAudioPlayer>
{
    protected const int SamplesBufferLength = 480; // VoiceChat.Networking.VoiceTransceiver._packageSize
    protected const int EncoderBufferLength = 512; // VoiceChat.Networking.VoiceTransceiver._encodedBuffer.Length
    internal static readonly List<BaseAudioPlayer> Players = [];

    private static int _idCounter;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseAudioPlayer" /> class.
    /// </summary>
    protected BaseAudioPlayer()
    {
        Id = _idCounter++;
        AudioTasks = new Queue<AudioTask>();
        _encoder = new OpusEncoder(OpusApplicationType.Voip);

        Players.Add(this);
    }

    /// <inheritdoc />
    public bool Equals(BaseAudioPlayer baseAudioPlayer)
    {
        return Id == baseAudioPlayer.Id;
    }

    /// <summary>
    ///     Launch an instance of the <see cref="Timing" /> coroutine that executes the player code.
    /// </summary>
    /// <returns>Has a new coroutine instance been created? (false when it's already running)</returns>
    public virtual bool RunCoroutine()
    {
        if (_coroutineHandler.IsRunning) return false;

        _coroutineHandler = Timing.RunCoroutine(MainCoroutine(), Segment.FixedUpdate);
        return true;
    }

    /// <summary>
    ///     Kill an instance of the <see cref="Timing" /> coroutine that executes the player code.
    /// </summary>
    /// <returns>Was the current coroutine instance killed? (false when it's already killed)</returns>
    public virtual bool KillCoroutine()
    {
        if (!_coroutineHandler.IsRunning) return false;

        Timing.KillCoroutines(_coroutineHandler);

        if (Players.Contains(this))
            Players.Remove(this);

        return true;
    }

    /// <summary>
    ///     Hook destroys method
    /// </summary>
    public abstract void DestroySelf();

    /// <summary>
    ///     Force start playing a new audio task (Bypassing the queue)
    /// </summary>
    /// <param name="audio">Audio to play</param>
    /// <param name="voiceChannel">Voice channel for playback</param>
    /// <param name="addDecibels">The number of decibels added to the volume of the playback</param>
    /// <param name="isMute">Mute the audio task?</param>
    /// <param name="isPause">Pause the audio task?</param>
    /// <param name="isLoop">Loop the audio task?</param>
    /// <returns>New instance of <see cref="AudioTask" />.</returns>
    /// <exception cref="ArgumentNullException" />
    public virtual AudioTask ForcePlay(
        IAudio audio,
        VoiceChatChannel voiceChannel = VoiceChatChannel.Proximity,
        float addDecibels = 0.0F,
        bool isMute = false,
        bool isPause = false,
        bool isLoop = false
    )
    {
        if (audio == null) throw new ArgumentNullException(nameof(audio));

        AudioTask audioTask = new(audio, voiceChannel, addDecibels, isMute, isPause, isLoop);
        CurrentAudioTask = audioTask;
        return audioTask;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is BaseAudioPlayer audioPlayer && audioPlayer.Equals(this);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Id;
    }

    public static bool operator ==(BaseAudioPlayer? a, BaseAudioPlayer? b)
    {
        return a?.Id == b?.Id;
    }

    public static bool operator !=(BaseAudioPlayer? a, BaseAudioPlayer? b)
    {
        return !(a == b);
    }

    #region Internal Methods

    protected virtual IEnumerator<float> MainCoroutine()
    {
        float[] samplesBuffer = new float[SamplesBufferLength];
        byte[] encodedBuffer = new byte[EncoderBufferLength];

        while (true)
        {
            // If the queue is empty and the current track is not forced, then we wait 100ms.
            while (AudioTasks.Count == 0 && CurrentAudioTask is null) yield return Timing.WaitForSeconds(0.1f);

            // If the track was not forced, and the queue was replenished, then we set track to the first task from the queue.
            CurrentAudioTask ??= AudioTasks.Dequeue();

            // If the track was skipped or does not have an audio installed (is this possible?), then we skip it.
            if (CurrentAudioTask.IsDone)
            {
                CurrentAudioTask = null;
                continue;
            }

            // We prepare the task for reading.
            CurrentAudioTask.Audio.ResetReadPosition();
            CurrentAudioTask.IsRunning = true;
            CurrentAudioTask.RunAt = DateTime.Now;

            // We're creating a variable for delays.
            long latency = 0;

            // While the task in playback and is running and (completed or skipped), we read its audio content and send it to clients.
            while (CurrentAudioTask is not null && CurrentAudioTask.IsRunning && !CurrentAudioTask.IsDone)
            {
                // While the task in playback and is paused, we wait 100ms.
                while (CurrentAudioTask is not null && CurrentAudioTask.IsPause)
                    yield return Timing.WaitForSeconds(0.1F);

                // If during the pause the task was removed from playback, then we stop the read cycle.
                if (CurrentAudioTask is null) break;

                // We add delay and read samples from the audio.
                latency += 10;
                Array.Clear(samplesBuffer, 0, SamplesBufferLength);
                CurrentAudioTask.Audio.ReadTo(samplesBuffer, ref CurrentAudioTask.LocalReadPosition,
                    SamplesBufferLength);

                // We add decibels to the playback volume.
                float volumeScale = Mathf.Pow(10.0F, CurrentAudioTask.AddDecibels / 20.0F);
                for (int i = 0; i < SamplesBufferLength; i++)
                    samplesBuffer[i] = CurrentAudioTask.IsMute ? 0 : samplesBuffer[i] * volumeScale;

                // We encode the samples and create a new voice message.
                int dataLength = _encoder.Encode(samplesBuffer, encodedBuffer);

                var messageSegment = SerializeAndPackToDataSegment(dataLength, encodedBuffer);

                // We send a voice message to every player on the server, except the host and speaker.
                foreach (ReferenceHub? referenceHub in ReferenceHub.AllHubs)
                {
                    if (!GetIsAllowedToPlay(referenceHub) || referenceHub?.connectionToClient == null) continue;

                    // We check the target for presence in the white and blacklists.
                    bool allowed = true;
                    if (CurrentAudioTask.Blacklist != null)
                        allowed &= !CurrentAudioTask.Blacklist.Contains(referenceHub);
                    if (CurrentAudioTask.Whitelist != null)
                        allowed &= CurrentAudioTask.Whitelist.Contains(referenceHub);
                    if (!allowed) continue;

                    referenceHub.connectionToClient.Send(messageSegment);
                }

                // We make a delay before the next iteration.
                if ((DateTime.Now - CurrentAudioTask.RunAt).TotalMilliseconds < latency - 10)
                    yield return Timing.WaitForOneFrame;

                // We check the task for completion.
                if (!CurrentAudioTask.Audio.IsReadEnded())
                    continue;

                if (CurrentAudioTask.IsLoop)
                {
                    // If the task has completed, and it is cyclic, then we reset the delays and task.
                    CurrentAudioTask.Audio.ResetReadPosition();
                    CurrentAudioTask.RunAt = DateTime.Now;
                    latency = 0;
                }
                else
                {
                    // If the task was not looped, then we finish reading it.
                    break;
                }
            }

            if (CurrentAudioTask is null)
                continue;

            // We reset the task parameters and remove it from the current playing task.
            CurrentAudioTask.IsRunning = false;
            CurrentAudioTask.IsDone = true;
            CurrentAudioTask = null;
        }

        // ReSharper disable once IteratorNeverReturns
    }

    protected virtual bool GetIsAllowedToPlay(ReferenceHub referenceHub)
    {
        return true;
    }

    protected abstract ArraySegment<byte> SerializeAndPackToDataSegment(int dataLength, byte[] dataBuffer,
        int channelId = 0);

    #endregion

    #region Properties

    /// <summary>
    ///     Queue of active audio tasks for playback.
    ///     <para>
    ///         A queue of audio tasks whose <see cref="AudioTask.IsDone" /> is false.
    ///     </para>
    /// </summary>
    public IEnumerable<AudioTask> ActiveAudioTasks
        => AudioTasks.Where(audioTask => !audioTask.IsDone);

    /// <summary>
    ///     Audio player ID, which is a unique number.
    /// </summary>
    public int Id { get; }

    /// <summary>
    ///     Queue of audio tasks for playback.
    /// </summary>
    public Queue<AudioTask> AudioTasks { get; }

    /// <summary>
    ///     The currently playing audio task.
    /// </summary>
    public AudioTask? CurrentAudioTask { get; protected set; }

    #endregion

    #region Fields

    private readonly OpusEncoder _encoder;

    private CoroutineHandle _coroutineHandler;

    #endregion

    #region API Methods

    /// <summary>
    ///     Add a new audio task to the player queue.
    /// </summary>
    /// <param name="audio">Audio to play</param>
    /// <param name="voiceChannel">Voice channel for playback</param>
    /// <param name="addDecibels">The number of decibels added to the volume of the playback</param>
    /// <param name="isMute">Mute the audio task?</param>
    /// <param name="isPause">Pause the audio task?</param>
    /// <param name="isLoop">Loop the audio task?</param>
    /// <returns>New instance of <see cref="AudioTask" />.</returns>
    /// <exception cref="ArgumentNullException" />
    public virtual AudioTask Play(
        IAudio audio,
        VoiceChatChannel voiceChannel = VoiceChatChannel.Proximity,
        float addDecibels = 0.0F,
        bool isMute = false,
        bool isPause = false,
        bool isLoop = false
    )
    {
        if (audio == null)
            throw new ArgumentNullException(nameof(audio));

        AudioTask audioTask = new(audio, voiceChannel, addDecibels, isMute, isPause, isLoop);
        AudioTasks.Enqueue(audioTask);
        return audioTask;
    }

    /// <summary>
    ///     Skip the audio task.
    /// </summary>
    /// <param name="audioTask">Audio task to skip</param>
    /// <exception cref="ArgumentNullException" />
    public virtual void Skip(AudioTask audioTask)
    {
        if (audioTask == null)
            throw new ArgumentNullException(nameof(audioTask));

        // Check if an audio task is related to this player
        if (!AudioTasks.Contains(audioTask) && CurrentAudioTask != audioTask) return;

        audioTask.Skip();
    }

    /// <summary>
    ///     Skip the current audio task.
    /// </summary>
    public virtual void Skip()
    {
        CurrentAudioTask?.Skip();
    }

    #endregion
}