using System;
using AdminToys;
using JetBrains.Annotations;
using Mirror;
using VoiceChat.Networking;

namespace Qurre.API.Addons.Audio;

[PublicAPI]
public class AudioPlayerSpeaker : BaseAudioPlayer
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AudioPlayerSpeaker" /> class.
    /// </summary>
    /// <param name="speakerToy"><see cref="AdminToys.SpeakerToy" /> of the audio source.</param>
    /// <exception cref="ArgumentNullException" />
    public AudioPlayerSpeaker(SpeakerToy speakerToy)
    {
        SpeakerToy = speakerToy ?? throw new ArgumentNullException(nameof(speakerToy));
    }

    public SpeakerToy SpeakerToy { get; }

    public override void DestroySelf()
    {
        base.KillCoroutine();

        try
        {
            NetworkServer.Destroy(SpeakerToy.gameObject);
        }
        catch
        {
            Log.Debug("Can not destroy audio player");
        }
    }

    protected override ArraySegment<byte> SerializeAndPackToDataSegment(int dataLength, byte[] dataBuffer,
        int channelId = 0)
    {
        using NetworkWriterPooled? writer = NetworkWriterPool.Get();
        AudioMessage message = new(
            SpeakerToy.ControllerId,
            dataBuffer,
            dataLength
        );

        NetworkMessages.Pack(message, writer);
        int maxMessageSize = NetworkMessages.MaxMessageSize(channelId);
        return writer.Position > maxMessageSize ? ArraySegment<byte>.Empty : writer.ToArraySegment();
    }
}