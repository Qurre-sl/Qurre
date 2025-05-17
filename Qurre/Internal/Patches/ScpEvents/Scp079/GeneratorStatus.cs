using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp079;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.ScpEvents.Scp079;

[HarmonyPatch(typeof(Scp079Recontainer), nameof(Scp079Recontainer.UpdateStatus))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class GeneratorStatus
{
    [HarmonyPrefix]
    private static void Call(int engagedGenerators)
    {
        new GeneratorStatusEvent(engagedGenerators, Scp079Recontainer.AllGenerators.Count).InvokeEvent();
    }
}