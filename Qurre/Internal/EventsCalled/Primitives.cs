using MEC;
using Qurre.API.Attributes;
using Qurre.API.Controllers;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Events;

#pragma warning disable IDE0051
namespace Qurre.Internal.EventsCalled
{
    static class Primitives
    {
        [EventMethod(RoundEvents.Start)]
        static void Started()
        {
            Primitive._allowStatic = true;

            foreach (Primitive prim in Primitive._cachedToSetStatic)
                prim.IsStatic = true;

            Primitive._cachedToSetStatic.Clear();
        }

        [EventMethod(RoundEvents.Restart)]
        [EventMethod(RoundEvents.Waiting)]
        static void Clear()
        {
            Primitive._allowStatic = false;
            Primitive._cachedToSetStatic.Clear();
        }

        [EventMethod(PlayerEvents.Join)]
        static void UpdateJoin(JoinEvent ev)
        {
            if (API.Round.Waiting)
                return;

            foreach (Primitive prim in Map.Primitives)
            {
                if (!prim.IsStatic)
                    continue;

                prim.IsStatic = false;
                Primitive._cachedToSetStatic.Add(prim);
            }

            Timing.CallDelayed(1f, () =>
            {
                foreach (Primitive prim in Primitive._cachedToSetStatic)
                    prim.IsStatic = true;
                Primitive._cachedToSetStatic.Clear();
            });
        }
    }
}
#pragma warning restore IDE0051