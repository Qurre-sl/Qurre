using HarmonyLib;
using PlayerRoles.PlayableScps.Scp079;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Scp.Scp079
{
    [HarmonyPatch(typeof(Scp079Recontainer), nameof(Scp079Recontainer.UpdateStatus))]
    static class GeneratorStatus
    {
        [HarmonyPrefix]
        static void Call(int engagedGenerators)
        {
            new GeneratorStatusEvent(engagedGenerators, Scp079Recontainer.AllGenerators.Count).InvokeEvent();
        }
    }
}