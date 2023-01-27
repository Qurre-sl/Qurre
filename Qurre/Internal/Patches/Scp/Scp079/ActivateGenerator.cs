using HarmonyLib;
using MapGeneration.Distributors;
using System;
using static MapGeneration.Distributors.Scp079Generator;

namespace Qurre.Internal.Patches.Scp.Scp079
{
    using Qurre.API;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerSetFlag))]
    static class ActivateGenerator
    {
        [HarmonyPrefix]
        static bool Call(Scp079Generator __instance, GeneratorFlags flag, bool state)
        {
            try
            {
                if (flag is not GeneratorFlags.Engaged)
                    return true;

                if (!state)
                    return true;

                var ev = new ActivateGeneratorEvent(__instance.GetGenerator());
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
}