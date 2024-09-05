using HarmonyLib;
using MapGeneration.Distributors;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Player.Interact
{
	using Qurre.API;
	using Qurre.Events.Structs;
	using Qurre.Internal.EventsManager;

	[HarmonyPatch(typeof(Locker), nameof(Locker.ServerInteract))]
	static class InteractLocker
	{
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
		{
			yield return new CodeInstruction(OpCodes.Ldarg_0); // Locker [instance]
			yield return new CodeInstruction(OpCodes.Ldarg_1); // ReferenceHub [ply]
			yield return new CodeInstruction(OpCodes.Ldarg_2); // byte [colliderId]
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(InteractLocker), nameof(InteractLocker.Invoke)));
			yield return new CodeInstruction(OpCodes.Ret);
		}

		// full rewrite for small optimization
		static void Invoke(Locker instance, ReferenceHub ply, byte colliderId)
		{
			try
			{
				if (colliderId >= instance.Chambers.Length)
					return;

				var chamber = instance.Chambers[colliderId];

				if (!chamber.CanInteract)
					return;

				bool allow = ply.serverRoles.BypassMode || instance.CheckPerms(chamber.RequiredPermissions, ply);

				var locker = instance.GetLocker();

				locker.Chambers.TryFind(out var chmbr, x => x.LockerChamber == chamber);

				InteractLockerEvent ev = new(ply.GetPlayer(), locker, chmbr, allow);
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
}