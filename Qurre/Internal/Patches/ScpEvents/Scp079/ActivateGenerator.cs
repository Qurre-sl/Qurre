using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API;
using Qurre.API.World;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.ScpEvents.Scp079;

using static Scp079Generator;

[HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerSetFlag))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class ActivateGenerator
{
    [HarmonyPrefix]
    private static bool Call(Scp079Generator __instance, GeneratorFlags flag, bool state)
    {
        try
        {
            if (flag is not GeneratorFlags.Engaged)
                return true;

            if (!state)
                return true;

            ActivateGeneratorEvent ev = new(__instance.GetGenerator());
            ev.InvokeEvent();

            if (ev.Allowed)
                Round.ActiveGenerators++;

            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <SCPs> {{Scp079}} [ActivateGenerator]: {e}\n{e.StackTrace}");
            return true;
        }
    }
}