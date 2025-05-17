using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace Qurre.API.Addons.Audio.Objects;

/// <summary>
///     Reads pem_f32le streams.
///     <para>
///         To convert: ffmpeg -y -i input.wav -f f32le -ac 1 -ar 48000 output.f32le
///     </para>
/// </summary>
[PublicAPI]
public class StreamAudio : IAudio
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="StreamAudio" /> class.
    /// </summary>
    /// <param name="stream"><see cref="System.IO.Stream" /> to read</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="NotSupportedException" />
    public StreamAudio(Stream stream)
    {
        Stream = stream ?? throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead) throw new NotSupportedException("Stream cannot be read!");
    }

    /// <summary>
    ///     The <see cref="System.IO.Stream" /> from which audio is read.
    /// </summary>
    public Stream Stream { get; }

    /// <inheritdoc />
    public virtual void ReadTo(float[] samplesArray, ref long readPos, long readLength)
    {
        Stream.Position = readPos;
        byte[] buffer = new byte[readLength * sizeof(float)];
        int count = Stream.Read(buffer, 0, buffer.Length);
        readPos += count;

        Buffer.BlockCopy(buffer, 0, samplesArray, 0, count);
    }

    /// <inheritdoc />
    public virtual bool IsReadEnded()
    {
        return Stream.Position >= Stream.Length;
    }

    /// <inheritdoc />
    public virtual void ResetReadPosition()
    {
        Stream.Position = 0;
    }

    /// <inheritdoc />
    public virtual float GetReadPercent()
    {
        return Mathf.Max((float)Stream.Position / Stream.Length, 1.0F);
    }
}