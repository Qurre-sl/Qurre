using Mirror;
using System;

namespace Qurre.API.Addons.Audio.Objects.Mirror
{
    /// <summary>
    /// <see cref="LocalConnectionToClient"/>, but without packets sending and disconnecting.
    /// </summary>
    public sealed class ZeroConnectionToClient : NetworkConnectionToClient
    {
        /// <inheritdoc/>
        public override string address { get; } = "localhost";

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeroConnectionToClient"/> class.
        /// </summary>
        public ZeroConnectionToClient() : base(0)
        {
        }

        /// <inheritdoc/>
        protected override void UpdatePing()
        {
           
        }

        /// <inheritdoc/>
        public override void Send(ArraySegment<byte> segment, int channelId = 0)
        {
        }

        /// <inheritdoc/>
        public override void Disconnect()
        {
        }
    }
}