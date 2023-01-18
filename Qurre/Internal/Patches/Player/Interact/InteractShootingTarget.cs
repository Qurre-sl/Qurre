using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using AdminToys;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Interact
{
    [HarmonyPatch(typeof(ShootingTarget), nameof(ShootingTarget.ServerInteract))]
    internal static class InteractShootingTarget
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
        {
            yield return new (OpCodes.Ldarg_0); // instance [ShootingTarget]
            yield return new (OpCodes.Ldarg_1); // ply [ReferenceHub]
            yield return new (OpCodes.Ldarg_2); // byte [colliderId]
            yield return new (OpCodes.Call, AccessTools.Method(typeof(InteractShootingTarget), nameof(Invoke)));
            yield return new (OpCodes.Ret);
        }

        private static void Invoke(ShootingTarget instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                InteractShootingTargetEvent ev = new (ply.GetPlayer(), API.Map.ShootingTargets.FirstOrDefault(x => x.Base == instance) ?? new (instance), (ShootingTarget.TargetButton)colliderId);

                if (!PermissionsHandler.IsPermitted(ply.serverRoles.Permissions, PlayerPermissions.FacilityManagement))
                {
                    ev.Allowed = false;
                }

                ev.InvokeEvent();

                if (!ev.Allowed)
                {
                    return;
                }

                colliderId = (byte)ev.Button;

                if (colliderId == 5)
                {
                    NetworkServer.Destroy(instance.gameObject);
                    return;
                }

                if (colliderId == 6)
                {
                    instance.Network_syncMode = !instance._syncMode;
                    return;
                }

                if (instance._syncMode && !ply.isLocalPlayer)
                {
                    instance.UseButton(ev.Button);
                    instance.RpcSendInfo(instance._maxHp, instance._autoDestroyTime);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Interact}} [ShootingTarget]: {e}\n{e.StackTrace}");
            }
        }
    }
}