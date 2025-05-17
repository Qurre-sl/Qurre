using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PlayerRoles.FirstPersonControl;

namespace Qurre.Internal.Patches.Misc.Fixes;

[HarmonyPatch(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.FixedUpdate))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
internal static class FixNullFPMM
{
    [HarmonyPrefix]
    private static bool Call(FirstPersonMovementModule __instance)
    {
        return __instance.Tracer is not null;
    }
}