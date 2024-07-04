using Hints;
using Mirror;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    using RoundRestarting;

    public sealed class Client
    {
        private readonly Player _player;
        internal Client(Player pl) => _player = pl;

        public HintDisplay HintDisplay => _player.ReferenceHub.hints;

        public void ShowHint(string text, float duration = 1f, HintEffect[] effect = null) =>
            HintDisplay.Show(new TextHint(text, new HintParameter[] { new StringHintParameter("") }, effect, duration));

        public Controllers.Broadcast Broadcast(string message, ushort time, bool instant = false) => Broadcast(time, message, instant);
        public Controllers.Broadcast Broadcast(ushort time, string message, bool instant = false)
        {
            Controllers.Broadcast bc = new(_player, message, time);
            _player.Broadcasts.Add(bc, instant);
            return bc;
        }

        public void SendConsole(string message, string color)
            => _player.ReferenceHub.gameConsoleTransmission.SendToClient(message, color);

        public void Disconnect(string reason = null)
            => ServerConsole.Disconnect(_player.GameObject, string.IsNullOrEmpty(reason) ? "" : reason);

        public void Reconnect(float timeout = 0.1f)
            => _player.Connection.Send(new RoundRestartMessage(RoundRestartType.FullRestart, timeout, 0, true, false));

        public void Redirect(ushort port, float timeout = 0.1f)
            => _player.Connection.Send(new RoundRestartMessage(RoundRestartType.RedirectRestart, timeout, port, true, false));

        public void DimScreen()
        {
            //RoundSummary.singleton.RpcDimScreen();

            int functionHashCode = -1745793588;
            var component = RoundSummary.singleton;
            var writer = NetworkWriterPool.Get();

            RpcMessage rpcMessage = new()
            {
                netId = component.netId,
                componentIndex = component.ComponentIndex,
                functionHash = (ushort)functionHashCode,
                payload = writer.ToArraySegment()
            };

            using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
            networkWriterPooled.Write(rpcMessage);
            _player.ReferenceHub.networkIdentity.connectionToClient.Send(rpcMessage, 0);

            NetworkWriterPool.Return(writer);
        }

        public void ShakeScreen(bool achieve = false)
        {
            //AlphaWarheadController.Singleton.RpcShake(achieve);

            int functionHashCode = 1208415683;
            var component = AlphaWarheadController.Singleton;
            var writer = NetworkWriterPool.Get();
            writer.WriteBool(achieve);

            RpcMessage rpcMessage = new()
            {
                netId = component.netId,
                componentIndex = component.ComponentIndex,
                functionHash = (ushort)functionHashCode,
                payload = writer.ToArraySegment()
            };

            using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
            networkWriterPooled.Write(rpcMessage);
            _player.ReferenceHub.networkIdentity.connectionToClient.Send(rpcMessage, 0);

            NetworkWriterPool.Return(writer);
        }
    }
}