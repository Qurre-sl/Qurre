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

        [EventMethod(PlayerEvents.Preauth)]
        static internal void Waiting(PreauthEvent _)
        {
            API.Log.Info($"userid: {_.UserId}; IP: {_.Ip}; Flags: {_.Flags}; Region: {_.Region}; Request: {_.Request}");
        }
    }
}