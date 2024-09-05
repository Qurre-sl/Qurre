using MEC;
using Qurre.API.Attributes;
using Qurre.API.Controllers;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Events;
using System.Collections.Generic;

#pragma warning disable IDE0051
namespace Qurre.Internal.EventsCalled
{
    static class Primitives
    {
        const string CoroutineName = "ToysCustomOptimizeUpdate";

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
            Primitive._nonstaticPrims.Clear();


            Timing.KillCoroutines(CoroutineName);

            if (API.Round.Waiting)
            {
                Timing.RunCoroutine(Coroutine(), Segment.LateUpdate, CoroutineName);
            }
        }

        [EventMethod(PlayerEvents.Join)]
        static void UpdateJoin(JoinEvent _)
        {
            if (API.Round.Waiting)
                return;

            foreach (Primitive prim in Map.Primitives)
            {
                if (!prim.IsStatic)
                    continue;

                prim.Base.NetworkIsStatic = false;
                Primitive._cachedToSetStatic.Add(prim);
            }

            Timing.CallDelayed(0.1f, () =>
            {
                foreach (Primitive prim in Primitive._cachedToSetStatic)
                    prim.Base.NetworkIsStatic = true;
                Primitive._cachedToSetStatic.Clear();
            });
        }


        static IEnumerator<float> Coroutine()
        {
            while (true)
            {
                foreach (Primitive prim in Primitive._nonstaticPrims)
                {
                    try
                    {
                        prim.Base.UpdatePositionServer();
                    }
                    catch { } // may flood, so disabled
                }

                foreach (LightPoint light in Map.Lights)
                {
                    try
                    {
                        light.Base.UpdatePositionServer();
                    }
                    catch { }
                }

                foreach (ShootingTarget shoot in Map.ShootingTargets)
                {
                    try
                    {
                        shoot.Base.UpdatePositionServer();
                    }
                    catch { }
                }

                yield return Timing.WaitForOneFrame;
            } // cycle end
        } // coroutine end


    } // class end
} // namespace end
#pragma warning restore IDE0051