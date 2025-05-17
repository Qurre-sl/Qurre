using System;
using JetBrains.Annotations;
using UnityEngine;
using VoiceChat.Networking;

namespace Qurre.API.Addons.Audio.Objects;

/// <summary>
///     Reads <see cref="VoiceChat.Networking.PlaybackBuffer" />.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="PlaybackAudio" /> class.
/// </remarks>
/// <param name="playbackBuffer">
///     <see cref="VoiceChat.Networking.PlaybackBuffer" /> to read
/// </param>
/// <exception cref="ArgumentNullException" />
[PublicAPI]
public class PlaybackAudio(PlaybackBuffer playbackBuffer) : IAudio
{
    /// <summary>
    ///     The <see cref="VoiceChat.Networking.PlaybackBuffer" /> from which audio is read.
    /// </summary>
    public PlaybackBuffer PlaybackBuffer { get; } =
        playbackBuffer ?? throw new ArgumentNullException(nameof(playbackBuffer));

    /// <inheritdoc />
    public virtual void ReadTo(float[] samplesArray, ref long readPos, long readLength)
    {
        PlaybackBuffer.ReadHead = readPos;
        readPos += readLength;

        PlaybackBuffer.ReadTo(samplesArray, readLength);
    }

    /// <inheritdoc />
    public virtual bool IsReadEnded()
    {
        return PlaybackBuffer.ReadHead >= PlaybackBuffer.Length;
    }

    /// <inheritdoc />
    public virtual void ResetReadPosition()
    {
        PlaybackBuffer.ReadHead = 0;
    }

    /// <inheritdoc />
    public virtual float GetReadPercent()
    {
        float readPercentFloat = (float)PlaybackBuffer.ReadHead / PlaybackBuffer.Length;
        return Mathf.Clamp01(readPercentFloat);
    }
}