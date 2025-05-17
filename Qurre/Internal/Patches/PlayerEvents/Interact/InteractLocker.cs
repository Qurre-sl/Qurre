using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API;
using Qurre.API.Entities;
using Qurre.API.Entities.Structures;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using Locker = MapGeneration.Distributors.Locker;

namespace Qurre.Internal.Patches.PlayerEvents.Interact;

[HarmonyPatch(typeof(Locker), nameof(Locker.ServerInteract))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class InteractLocker
{
    // Полностью перенести события взаимодействия с Locker-ами на базу LabApi
    /*
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
            if (!EntityManager.TryGet(instance, out ILocker? locker))
                return;

            if (!instance.Chambers.TryGet<LockerChamber>(colliderId, out var lockerChamber))
                return;

            if (!lockerChamber.CanInteract)
                return;

            var player = ply.GetPlayer();

            if (player is null)
                return;

            var isAllowed = instance.CheckTogglePerms(colliderId, ply, out var permissionUsed) || ply.serverRoles.BypassMode;

            InteractLockerEvent ev = new(player, locker, lockerChamber, isAllowed);
            ev.InvokeEvent();

            if (!ev.Allowed)
            {
                instance.RpcPlayDenied(colliderId);
                return;
            }

            lockerChamber.SetDoor(!lockerChamber.IsOpen, instance._grantedBeep);
            instance.RefreshOpenedSyncvar();
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Interact}} [Locker]: {e}\n{e.StackTrace}");
        }
    }
    */
}