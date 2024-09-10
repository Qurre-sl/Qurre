using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using RoundRestarting;

namespace Qurre.Internal.Patches.RoundEvents;

[HarmonyPatch(typeof(RoundRestart), nameof(RoundRestart.InitiateRoundRestart))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Restart
{
    [HarmonyPrefix]
    private static void Call()
    {
        new RoundRestartEvent().InvokeEvent();
    }
}