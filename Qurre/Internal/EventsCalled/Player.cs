using Qurre.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.Internal.EventsCalled
{
    internal class Player
    {
        [EventMethod(PlayerEvents.Death)]
        static internal void PatchDead()
        {

        }
    }
}