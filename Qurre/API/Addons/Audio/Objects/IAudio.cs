using JetBrains.Annotations;

namespace Qurre.API.Addons.Audio.Objects;

/// <summary>
///     Audio reading interface.
/// </summary>
[PublicAPI]
public interface IAudio
{
    /// <summary>
    ///     Read the following audio fragment.
    /// </summary>
    /// <param name="samplesArray">Output samples buffer array</param>
    /// <param name="readPos">Position from which samples should be read</param>
    /// <param name="readLength">Length indicating how many samples to read</param>
    void ReadTo(float[] samplesArray, ref long readPos, long readLength);

    /// <summary>
    ///     Check if the reading has been completed.
    /// </summary>
    bool IsReadEnded();

    /// <summary>
    ///     Reset reading position.
    /// </summary>
    void ResetReadPosition();

    /// <summary>
    ///     Get the audio read percentage. (from 0 to 1, with floating point)
    /// </summary>
    /// <returns>Read percentage.</returns>
    float GetReadPercent();
}