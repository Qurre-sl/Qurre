using HarmonyLib;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using RoundRestarting;

namespace Qurre.Internal.Patches.Round
{
    [HarmonyPatch(typeof(RoundRestart), nameof(RoundRestart.InitiateRoundRestart))]
    internal static class Restart
    {
        [HarmonyPrefix]
        private static void Call() => new RoundRestartEvent().InvokeEvent();
    }
}