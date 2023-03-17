using HarmonyLib;
using PlayerRoles.FirstPersonControl;

namespace Qurre.Internal.Patches.Misc.Fixes
{
    [HarmonyPatch(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.FixedUpdate))]
    static class FixNullFPMM
    {
        [HarmonyPrefix]
        static bool Call(FirstPersonMovementModule __instance) => __instance.Tracer is not null;
    }
}