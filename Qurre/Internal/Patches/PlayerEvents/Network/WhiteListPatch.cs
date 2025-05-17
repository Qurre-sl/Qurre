using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Network;

[HarmonyPatch(typeof(WhiteList), nameof(WhiteList.IsWhitelisted))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class WhiteListPatch
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // userId [string]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WhiteListPatch), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static bool Invoke(string userId)
    {
        try
        {
            bool allow = !WhiteList.WhitelistEnabled || WhiteList.Users.Contains(userId);

            CheckWhiteListEvent ev = new(userId, allow);
            ev.InvokeEvent();

            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Network}} [WhiteList]: {e}\n{e.StackTrace}");
            return false;
        }
    }
}