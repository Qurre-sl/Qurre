using HarmonyLib;

namespace Qurre.Internal.Patches.Round
{
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.UserCode_RpcRoundStarted))]
    static class Start
    {
        [HarmonyPostfix]
        static void Call() => new RoundStartedEvent().InvokeEvent();
    }
}