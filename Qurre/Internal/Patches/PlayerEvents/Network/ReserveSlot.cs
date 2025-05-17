using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using CentralAuth;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Network;

[HarmonyPatch(typeof(ReservedSlot), nameof(ReservedSlot.HasReservedSlot))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class ReserveSlot
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // userId [string]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ReserveSlot), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static bool Invoke(string userId)
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