using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using MEC;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.World;
using Respawning;

namespace Qurre.Internal.Patches.Misc.Fixes;

[HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.PlayCassieAnnouncement))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class CassieController
{
    [HarmonyPrefix]
    private static bool Call(string words, bool makeHold, bool makeNoise)
    {
        if (Cassie.Lock)
            return false;

        try
        {
            foreach (Cassie _ in Map.Cassies)
                if (_.Message == words && _.Hold == makeHold && _.Noise == makeNoise)
                {
                    Map.Cassies.Remove(_);
                    Timing.CallDelayed(NineTailedFoxAnnouncer.singleton.CalculateDuration(words), Cassie.End);
                    return true;
                }

            Map.Cassies.Add(new Cassie(words, makeHold, makeNoise), true);
            return false;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Misc> {{Fixes}} [CassieController]: {e}\n{e.StackTrace}");
        }

        return true;
    }
}