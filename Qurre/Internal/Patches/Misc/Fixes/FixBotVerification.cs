using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace Qurre.Internal.Patches.Misc.Fixes;

[HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.HandlePlayerJoin))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class FixBotVerification
{
    [HarmonyPrefix]
    private static bool Call(ReferenceHub rh)
    {
        return !(string.IsNullOrEmpty(rh.authManager.UserId) || rh.authManager.UserId.EndsWith("@bot"));
    }
}