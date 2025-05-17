using System;
using JetBrains.Annotations;
using Qurre.API.Addons.Audio.Objects;
using VoiceChat;

namespace Qurre.API.Addons.Audio;

/// <summary>
///     Audio task of playback.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="AudioTask" /> class.
/// </remarks>
/// <param name="audio">Audio to play</param>
/// <param name="voiceChannel">Voice channel for playback</param>
/// <param name="addDecibels">The number of decibels added to the volume of the playback</param>
/// <param name="isMute">Mute the audio task?</param>
/// <param name="isPause">Pause the audio task?</param>
/// <param name="isLoop">Loop the audio task?</param>
/// <exception cref="ArgumentNullException" />
[PublicAPI]
public sealed class AudioTask(
    IAudio audio,
    VoiceChatChannel voiceChannel,
    float addDecibels = 0.0F,
    bool isMute = false,
    bool isPause = false,
    bool isLoop = false
) : IEquatable<AudioTask>
{
    private static int _idCounter;

    #region Fields

    internal long LocalReadPosition;

    #endregion

    #region API Methods

    /// <summary>
    ///     Skip the audio task.
    /// </summary>
    public void Skip()
    {
        IsRunning = false;
        IsDone = true;
    }

    #endregion

    #region Basic

    /// <inheritdoc />
    public bool Equals(AudioTask other)
    {
        return Id == other.Id;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is AudioTask audioTask && audioTask.Equals(this);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Id;
    }

    public static bool operator ==(AudioTask? a, AudioTask? b)
    {
        return a?.Id == b?.Id;
    }

    public static bool operator !=(AudioTask? a, AudioTask? b)
    {
        return !(a == b);
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Audio read as a percentage. (from 0 to 1, with floating point)
    /// </summary>
    public float ReadPercent => Audio.GetReadPercent();

    /// <summary>
    ///     Audio task ID, which is a unique number.
    /// </summary>
    public int Id { get; } = _idCounter++;

    /// <summary>
    ///     Date the audio task was created.
    /// </summary>
    public DateTime CreatedAt { get; } = DateTime.Now;

    /// <summary>
    ///     Audio file of the task.
    /// </summary>
    public IAudio Audio { get; } = audio ?? throw new ArgumentNullException(nameof(audio));

    /// <summary>
    ///     Whitelists for audio playback.
    /// </summary>
    public AudioTaskWhitelist Whitelist { get; } = new();

    /// <summary>
    ///     Blacklists for audio playback.
    /// </summary>
    public AudioTaskBlacklist Blacklist { get; } = new();

    /// <summary>
    ///     Is the task running?
    /// </summary>
    public bool IsRunning { get; internal set; }

    /// <summary>
    ///     Is the task completed?
    ///     <para>
    ///         Whether the task is skipped or has it already be played.
    ///     </para>
    /// </summary>
    public bool IsDone { get; internal set; }

    /// <summary>
    ///     Date the audio task was ran.
    /// </summary>
    public DateTime RunAt { get; internal set; }

    /// <summary>
    ///     Current reading position of audio.
    /// </summary>
    public long ReadPosition
    {
        get => LocalReadPosition;
        set => LocalReadPosition = Math.Max(value, 0);
    }

    /// <summary>
    ///     The number of decibels added to the volume of the playback.
    /// </summary>
    public float AddDecibels { get; set; } = addDecibels;

    /// <summary>
    ///     The voice channel into which audio is played.
    /// </summary>
    public VoiceChatChannel VoiceChannel { get; set; } = voiceChannel;

    /// <summary>
    ///     Should the audio be muted?
    /// </summary>
    public bool IsMute { get; set; } = isMute;

    /// <summary>
    ///     Should the audio be paused?
    /// </summary>
    public bool IsPause { get; set; } = isPause;

    /// <summary>
    ///     Should the audio be looped?
    /// </summary>
    public bool IsLoop { get; set; } = isLoop;

    #endregion
}