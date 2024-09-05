using HarmonyLib;

namespace Qurre.Internal.Patches.Round
{
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.ForceRoundStart))]
    static class ForceStart
    {
        [HarmonyPrefix]
        static void Call() => new RoundForceStartEvent().InvokeEvent();
    }
}