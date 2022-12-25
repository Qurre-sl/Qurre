using Qurre.API.Attributes;
using Qurre.Events;
using Qurre.Events.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qurre.Internal.EventsCalled
{
    static internal class Player
    {
        [EventMethod(PlayerEvents.Death)]
        static internal void PatchDead()
        {

        }

        [EventMethod(PlayerEvents.CheckReserveSlot)]
        static internal void CheckSlot(CheckReserveSlotEvent ev)
        {
            API.Log.Custom(ev.Allowed);
            ev.Allowed = false;
        }
    }
}