using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.Events.Structs.Scp.Scp173
{
    public class BlinkEvent : IBaseEvent
    {
        public uint EventId => ScpEvents.Blink;
        public bool Allowed { get; set; }
    }
}
