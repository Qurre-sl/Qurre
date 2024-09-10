using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.RoundEvents;

[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.UserCode_RpcRoundStarted))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Start
{
    [HarmonyPostfix]
    private static void Call()
    {
        new RoundStartedEvent().InvokeEvent();
    }
}