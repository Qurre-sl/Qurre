using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hints;
using Mirror;

namespace Qurre.API.Classification.Player
{
    using Qurre.API;
    using Qurre.API.Controllers;

    public class Client
    {
        private readonly Player _player;
        internal Client(Player pl) => _player = pl;

        public HintDisplay HintDisplay => _player.ReferenceHub.hints;

        public void ShowHint(string text, float duration = 1f, HintEffect[] effect = null) =>
            HintDisplay.Show(new TextHint(text, new HintParameter[] { new StringHintParameter("") }, effect, duration));

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
        public Controllers.Broadcast Broadcast(string message, ushort time, bool instant = false) => Broadcast(time, message, instant);
        public Controllers.Broadcast Broadcast(ushort time, string message, bool instant = false)
        {
            Controllers.Broadcast bc = new(_player, message, time);
            _player.Broadcasts.Add(bc, instant);
            return bc;
        }
    }
}