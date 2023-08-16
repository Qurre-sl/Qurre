using Hints;
using Mirror;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;

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
        {
            try { _player.ClassManager.ConsolePrint(message, color); }
            catch { _player.ReferenceHub.GetComponent<GameConsoleTransmission>().SendToClient(_player.Connection, message, color); }
        }

        public void Disconnect(string reason = null)
            => ServerConsole.Disconnect(_player.GameObject, string.IsNullOrEmpty(reason) ? "" : reason);

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
            RpcMessage rpcMessage2 = rpcMessage;
            using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
            networkWriterPooled.Write(rpcMessage2);
            _player.ReferenceHub.networkIdentity.connectionToClient.BufferRpc(rpcMessage, 0);

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
            RpcMessage rpcMessage2 = rpcMessage;
            using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
            networkWriterPooled.Write(rpcMessage2);
            _player.ReferenceHub.networkIdentity.connectionToClient.BufferRpc(rpcMessage, 0);

            NetworkWriterPool.Return(writer);
        }
    }
}