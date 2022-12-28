using HarmonyLib;
using RoundRestarting;

namespace Qurre.Internal.Patches.Round
{
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(RoundRestart), nameof(RoundRestart.InitiateRoundRestart))]
    static class Restart
    {
        [HarmonyPrefix]
        static void Call() => new RoundRestartEvent().InvokeEvent();
    }
}