using HarmonyLib;
using MEC;
using Respawning;
using System;

namespace Qurre.Internal.Patches.Misc.Fixes
{
    using Qurre.API;
    using Qurre.API.Controllers;

    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.PlayCassieAnnouncement))]
    static class CassieController
    {
        [HarmonyPrefix]
        static bool Call(string words, bool makeHold, bool makeNoise)
        {
            if (Cassie.Lock)
                return false;

            try
            {
                foreach (Cassie _ in Map.Cassies)
                {
                    if (_.Message == words && _.Hold == makeHold && _.Noise == makeNoise)
                    {
                        Map.Cassies.Remove(_);
                        Timing.CallDelayed(NineTailedFoxAnnouncer.singleton.CalculateDuration(words), () => Cassie.End());
                        return true;
                    }
                }
                Map.Cassies.Add(new(words, makeHold, makeNoise), true);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Misc> {{Fixes}} [CassieController]: {e}\n{e.StackTrace}");
            }

            return true;
        }
    }
}