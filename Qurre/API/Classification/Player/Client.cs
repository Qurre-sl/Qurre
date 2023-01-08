using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.API.Classification.Player
{
    using Hints;
    using Mirror;
    using Qurre.API;
    public class Client
    {
        private readonly Player _player;
        internal Client(Player pl) => _player = pl;
        public void DimScreen()
        {
            var component = RoundSummary.singleton;
            var writer = NetworkWriterPool.GetWriter();
            var msg = new RpcMessage
            {
                netId = component.netId,
                componentIndex = component.ComponentIndex,
                functionHash = typeof(RoundSummary).FullName.GetStableHashCode() * 503 + "RpcDimScreen".GetStableHashCode(),
                payload = writer.ToArraySegment()
            };
            _player.Connection.Send(msg);
            NetworkWriterPool.Recycle(writer);
        }

        public void ShakeScreen(bool achieve = false)
        {
            var component = AlphaWarheadController.Singleton;
            var writer = NetworkWriterPool.GetWriter();
            writer.WriteBoolean(achieve);
            var msg = new RpcMessage
            {
                netId = component.netId,
                componentIndex = component.ComponentIndex,
                functionHash = typeof(AlphaWarheadController).FullName.GetStableHashCode() * 503 + "RpcShake".GetStableHashCode(),
                payload = writer.ToArraySegment()
            };
            _player.Connection.Send(msg);
            NetworkWriterPool.Recycle(writer);
        }
        public HintDisplay HintDisplay => _player.ReferenceHub.hints;
        public void ShowHint(string text, float duration = 1f, HintEffect[] effect = null) =>
            HintDisplay.Show(new TextHint(text, new HintParameter[] { new StringHintParameter("") }, effect, duration));
    }
}