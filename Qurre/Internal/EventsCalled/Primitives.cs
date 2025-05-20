using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MEC;
using Qurre.API.Attributes;
using Qurre.API.Controllers;
using Qurre.API.World;
using Qurre.Events;

namespace Qurre.Internal.EventsCalled;

internal static class Primitives
{
    private const string CoroutineName = "ToysCustomOptimizeUpdate";

    [EventMethod(RoundEvents.Start)]
    private static void Started()
    {
        Primitive.AllowStatic = true;

        foreach (Primitive prim in Primitive.CachedToSetStatic)
            prim.IsStatic = true;

        Primitive.CachedToSetStatic.Clear();
    }

    [EventMethod(RoundEvents.Restart)]
    [EventMethod(RoundEvents.Waiting)]
    private static void Clear()
    {
        Primitive.AllowStatic = false;
        Primitive.CachedToSetStatic.Clear();
        Primitive.NonStaticPrims.Clear();


        Timing.KillCoroutines(CoroutineName);

        if (API.World.Round.Waiting) Timing.RunCoroutine(ToysUpdate(), Segment.LateUpdate, CoroutineName);
    }

    [EventMethod(PlayerEvents.Join)]
    private static void UpdateJoin()
    {
        if (API.World.Round.Waiting)
            return;

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (Primitive prim in Map.Primitives)
        {
            if (!prim.IsStatic)
                continue;

            prim.Base.NetworkIsStatic = false;
            Primitive.CachedToSetStatic.Add(prim);
        }

        Timing.CallDelayed(0.1f, () =>
        {
            foreach (Primitive prim in Primitive.CachedToSetStatic)
                prim.Base.NetworkIsStatic = true;
            Primitive.CachedToSetStatic.Clear();
        });
    }


    [SuppressMessage("ReSharper", "IteratorNeverReturns")]
    private static IEnumerator<float> ToysUpdate()
    {
        while (true)
        {
            foreach (Primitive prim in Primitive.NonStaticPrims)
                try
                {
                    prim.Base.UpdatePositionServer();
                }
                catch
                {
                    // may flood, so disabled
                }

            foreach (LightPoint light in Map.Lights)
                try
                {
                    light.Base.UpdatePositionServer();
                }
                catch
                {
                    // may flood, so disabled
                }

            foreach (ShootingTarget shoot in Map.ShootingTargets)
                try
                {
                    shoot.Base.UpdatePositionServer();
                }
                catch
                {
                    // may flood, so disabled
                }

            foreach (Speaker speaker in Map.Speakers)
                try
                {
                    speaker.Base.UpdatePositionServer();
                }
                catch
                {
                    // may flood, so disabled
                }

            yield return Timing.WaitForOneFrame;
        } // while end
    } // coroutine end
}