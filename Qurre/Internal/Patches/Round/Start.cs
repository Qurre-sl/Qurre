using HarmonyLib;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Round
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.UserCode_RpcRoundStarted))]
    internal static class Start
    {
        [HarmonyPostfix]
        private static void Call() => new RoundStartedEvent().InvokeEvent();
    }
}