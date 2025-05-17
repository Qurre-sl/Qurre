using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;
using ShootingTarget = AdminToys.ShootingTarget;

namespace Qurre.Internal.Patches.PlayerEvents.Interact;

[HarmonyPatch(typeof(ShootingTarget), nameof(ShootingTarget.ServerInteract))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class InteractShootingTarget
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> _)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0); // instance [ShootingTarget]
        yield return new CodeInstruction(OpCodes.Ldarg_1); // ply [ReferenceHub]
        yield return new CodeInstruction(OpCodes.Ldarg_2); // byte [colliderId]
        yield return new CodeInstruction(OpCodes.Call,
            AccessTools.Method(typeof(InteractShootingTarget), nameof(Invoke)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static void Invoke(ShootingTarget instance, ReferenceHub ply, byte colliderId)
    {
        try
        {
            Player? player = ply.GetPlayer();

            if (player is null)
                return;

            InteractShootingTargetEvent ev = new(player, instance.GetShootingTarget(),
                (ShootingTarget.TargetButton)colliderId);

            if (!PermissionsHandler.IsPermitted(ply.serverRoles.Permissions, PlayerPermissions.FacilityManagement))
                ev.Allowed = false;

            ev.InvokeEvent();

            if (!ev.Allowed)
                return;

            colliderId = (byte)ev.Button;

            switch (colliderId)
            {
                case 5:
                    NetworkServer.Destroy(instance.gameObject);
                    return;
                case 6:
                    instance.Network_syncMode = !instance._syncMode;
                    return;
            }

            if (!instance._syncMode || ply.isLocalPlayer)
                return;

            instance.UseButton(ev.Button);
            instance.RpcSendInfo(instance._maxHp, instance._autoDestroyTime);
        }
        catch (Exception e)
        {
            Log.Error($"Patch Error - <Player> {{Interact}} [ShootingTarget]: {e}\n{e.StackTrace}");
        }
    }
}