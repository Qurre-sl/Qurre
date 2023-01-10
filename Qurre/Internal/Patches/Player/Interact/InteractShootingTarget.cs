using AdminToys;
using HarmonyLib;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Player.Interact
{
	using Qurre.API;
	using Qurre.Events.Structs;
	using Qurre.Internal.EventsManager;

	[HarmonyPatch(typeof(ShootingTarget), nameof(ShootingTarget.ServerInteract))]
	static class InteractShootingTarget
	{
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
		{
			yield return new CodeInstruction(OpCodes.Ldarg_0); // instance [ShootingTarget]
			yield return new CodeInstruction(OpCodes.Ldarg_1); // ply [ReferenceHub]
			yield return new CodeInstruction(OpCodes.Ldarg_2); // byte [colliderId]
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(InteractShootingTarget), nameof(InteractShootingTarget.Invoke)));
			yield return new CodeInstruction(OpCodes.Ret);
		}

		static void Invoke(ShootingTarget instance, ReferenceHub ply, byte colliderId)
		{
			try
			{
				InteractShootingTargetEvent ev = new(ply.GetPlayer(), Map.ShootingTargets.FirstOrDefault(x => x.Base == instance) ??
					new(instance), (ShootingTarget.TargetButton)colliderId);

				if (!PermissionsHandler.IsPermitted(ply.serverRoles.Permissions, PlayerPermissions.FacilityManagement))
					ev.Allowed = false;

				ev.InvokeEvent();

				if (!ev.Allowed)
					return;

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