using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mirror;
using VoiceChat;
using VoiceChat.Networking;

namespace Qurre.API.Addons.Audio;

/// <summary>
///     Audio player for playing sounds on behalf of an entity via SCP:SL voice chat.
/// </summary>
[PublicAPI]
public class AudioPlayerBot : BaseAudioPlayer
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AudioPlayerBot" /> class.
    /// </summary>
    /// <param name="referenceHub"><see cref="global::ReferenceHub" /> of the audio source.</param>
    /// <exception cref="ArgumentNullException" />
    public AudioPlayerBot(ReferenceHub referenceHub)
    {
        ReferenceHub = referenceHub ?? throw new ArgumentNullException(nameof(referenceHub));
    }

    internal new static IEnumerable<AudioPlayerBot> Players => BaseAudioPlayer.Players.OfType<AudioPlayerBot>();

    /// <summary>
    ///     <see cref="global::ReferenceHub" /> of the entity on whose behalf the playback is taking place.
    /// </summary>
    public ReferenceHub ReferenceHub { get; }

    public override void DestroySelf()
    {
        base.KillCoroutine();

        try
        {
            NetworkServer.Destroy(ReferenceHub.gameObject);
        }
        catch
        {
            Log.Debug("Can not destroy audio player");
        }
    }

    protected override bool GetIsAllowedToPlay(ReferenceHub referenceHub)
    {
        return referenceHub != ReferenceHub;
    }

    protected override ArraySegment<byte> SerializeAndPackToDataSegment(int dataLength, byte[] dataBuffer,
        int channelId = 0)
    {
        using NetworkWriterPooled? writer = NetworkWriterPool.Get();
        VoiceMessage message = new(
            ReferenceHub,
            CurrentAudioTask?.VoiceChannel ?? VoiceChatChannel.None,
            dataBuffer,
            dataLength,
            false
        );

        NetworkMessages.Pack(message, writer);
        int maxMessageSize = NetworkMessages.MaxMessageSize(channelId);
        return writer.Position > maxMessageSize ? ArraySegment<byte>.Empty : writer.ToArraySegment();
    }
}