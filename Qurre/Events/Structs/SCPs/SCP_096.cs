using Qurre.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scp096 = PlayableScps.Scp096;

namespace Qurre.Events.Structs.SCPs
{
    public class AddTargetEvent : IBaseEvent
    {
        public AddTargetEvent(Scp096 scp096, Player player, Player target, bool allowed = true)
        {
            Scp096 = scp096;
            Player = player;
            Target = target;
            Allowed = allowed;
        }
        public Scp096 Scp096 { get; }
        public Player Player { get; }
        public Player Target { get; }
        public bool Allowed { get; set; }

        public uint EventId { get; } = ScpEvents.AddTarget;
    }
}
