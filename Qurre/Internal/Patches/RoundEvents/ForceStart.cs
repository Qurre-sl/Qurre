using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.RoundEvents;

[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.ForceRoundStart))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class ForceStart
{
    [HarmonyPrefix]
    private static void Call()
    {
        new RoundForceStartEvent().InvokeEvent();
    }
}