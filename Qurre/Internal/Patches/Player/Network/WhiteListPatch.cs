namespace Qurre.Internal.Patches.Player.Network;

using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

[HarmonyPatch(typeof(WhiteList), nameof(WhiteList.IsWhitelisted))]
static class WhiteListPatch
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // userId [string]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WhiteListPatch), nameof(WhiteListPatch.Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    static bool Invoke(string userId)
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