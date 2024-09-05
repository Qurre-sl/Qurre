namespace Qurre.Internal.Patches.Player.Network;

using CentralAuth;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

[HarmonyPatch(typeof(ReservedSlot), nameof(ReservedSlot.HasReservedSlot))]
static class ReserveSlot
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // userId [string]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ReserveSlot), nameof(ReserveSlot.Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    static bool Invoke(string userId)
    {
        try
        {
            bool allow = ReservedSlot.Users.Contains(userId.Trim()) || !PlayerAuthenticationManager.OnlineMode;

            CheckReserveSlotEvent ev = new(userId, allow);
            ev.InvokeEvent();

            return ev.Allowed;
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Network}} [ReserveSlot]: {e}\n{e.StackTrace}");
            return false;
        }
    }
}