using HarmonyLib;

namespace Qurre.Internal.Patches.Misc.Fixes
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.HandlePlayerJoin))]
    static class FixBotVerification
    {
        [HarmonyPrefix]
        static bool Call(ReferenceHub rh)
            => !string.IsNullOrEmpty(rh.authManager.UserId);
    }
}