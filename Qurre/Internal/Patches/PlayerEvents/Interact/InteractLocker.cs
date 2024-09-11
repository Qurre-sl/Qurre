using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.PlayerEvents.Interact;

[HarmonyPatch(typeof(Locker), nameof(Locker.ServerInteract))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class InteractLocker
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // Locker [instance]
        yield return new CodeInstruction(OpCodes.Ldarg_1); // ReferenceHub [ply]
        yield return new CodeInstruction(OpCodes.Ldarg_2); // byte [colliderId]
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(InteractLocker), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    // full rewrite for small optimization
    private static void Invoke(Locker instance, ReferenceHub ply, byte colliderId)
    {
        try
        {
            if (colliderId >= instance.Chambers.Length)
                return;

            LockerChamber? chamber = instance.Chambers[colliderId];

            if (!chamber.CanInteract)
                return;

            Player? player = ply.GetPlayer();

            if (player is null)
                return;

            bool allow = ply.serverRoles.BypassMode || instance.CheckPerms(chamber.RequiredPermissions, ply);

            API.Controllers.Locker locker = instance.GetLocker();

            locker.Chambers.TryFind(out var chamber2, x => x.LockerChamber == chamber);

            InteractLockerEvent ev = new(player, locker, chamber2, allow);
            ev.InvokeEvent();

            if (!ev.Allowed)
            {
                instance.RpcPlayDenied(colliderId);
                return;
            }

            chamber.SetDoor(!chamber.IsOpen, instance._grantedBeep);
            instance.RefreshOpenedSyncvar();
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Interact}} [Locker]: {e}\n{e.StackTrace}");
        }
    }
}