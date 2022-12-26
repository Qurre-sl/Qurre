using Qurre.API.Attributes;
using Qurre.Events;

namespace Qurre.Internal.EventsCalled
{
    static class Round
    {
        [EventMethod(RoundEvents.Waiting)]
        static internal void Waiting()
        {
            API.Extensions.DamagesCached.Clear();
        }
    }
}