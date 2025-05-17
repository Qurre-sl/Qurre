using System;
using Mirror;

namespace Qurre.API.Addons.Audio.Objects.Mirror;

/// <summary>
///     <see cref="LocalConnectionToClient" />, but without packets sending and disconnecting.
/// </summary>
public sealed class ZeroConnectionToClient : NetworkConnectionToClient
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ZeroConnectionToClient" /> class.
    /// </summary>
    public ZeroConnectionToClient() : base(0)
    {
    }

    /// <inheritdoc />
    public override string address => "127.0.0.1";

    /// <inheritdoc />
    public override void UpdatePing()
    {
    }

    /// <inheritdoc />
    public override void Send(ArraySegment<byte> segment, int channelId = 0)
    {
    }

    /// <inheritdoc />
    public override void Disconnect()
    {
    }
}