using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using MapGeneration.Distributors;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Player.Interact
{
	using Qurre.API;
	using Qurre.API.Objects;
	using Qurre.Events.Structs;
	using Qurre.Internal.EventsManager;

	[HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerInteract))]
	static class InteractGenerator
	{
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions)
		{
			yield return new CodeInstruction(OpCodes.Ldarg_0); // Scp079Generator [instance]
			yield return new CodeInstruction(OpCodes.Ldarg_1); // ReferenceHub [ply]
			yield return new CodeInstruction(OpCodes.Ldarg_2); // byte [colliderId]
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(InteractGenerator), nameof(InteractGenerator.Invoke)));
			yield return new CodeInstruction(OpCodes.Ret);
		}

		static void Invoke(Scp079Generator instance, ReferenceHub ply, byte colliderId)
		{
			try
			{
				if (instance._cooldownStopwatch.IsRunning &&
					instance._cooldownStopwatch.Elapsed.TotalSeconds < instance._targetCooldown)
					return;

				if (colliderId != 0 && !instance.HasFlag(instance._flags, Scp079Generator.GeneratorFlags.Open))
					return;

				instance._cooldownStopwatch.Stop();

				var pl = ply.GetPlayer();
				if (pl is null) return;

				switch (colliderId)
				{
					case 0: // Open, Close, Unlock
						{
							if (instance.HasFlag(instance._flags, Scp079Generator.GeneratorFlags.Unlocked))
							{
								bool opened = instance.HasFlag(instance._flags, Scp079Generator.GeneratorFlags.Open);

								InteractGeneratorEvent ev = new(pl, instance.GetGenerator(),
									opened ? GeneratorStatus.CloseDoor : GeneratorStatus.OpenDoor);
								ev.InvokeEvent();

								if (ev.Allowed)
									instance.ServerSetFlag(Scp079Generator.GeneratorFlags.Open, !opened);
								else
									instance.RpcDenied();

								instance._targetCooldown = instance._doorToggleCooldownTime;
							}
							else
							{
								InteractGeneratorEvent ev = new(pl, instance.GetGenerator(), GeneratorStatus.Unlock);

								if (ply.serverRoles.BypassMode ||
									(ply.inventory.CurInstance is not null && ply.inventory.CurInstance is KeycardItem card
									&& card.Permissions.HasFlagFast(instance._requiredPermission)))
									ev.Allowed = true;
								else
									ev.Allowed = false;

								ev.InvokeEvent();

								if (ev.Allowed)
								{
									instance.ServerSetFlag(Scp079Generator.GeneratorFlags.Unlocked, true);
									instance.ServerGrantTicketsConditionally(new Footprinting.Footprint(ply), 0.5f);
								}
								else
								{
									instance._targetCooldown = instance._unlockCooldownTime;
									instance.RpcDenied();
								}
							}
							break;
						}
					case 1: // Activate / Disable
						if ((ply.IsHuman() || instance.Activating) && !instance.Engaged)
						{
							InteractGeneratorEvent ev = new(pl, instance.GetGenerator(),
								instance.Activating ? GeneratorStatus.Deactivate : GeneratorStatus.Activate);
							ev.InvokeEvent();

							if (!ev.Allowed)
								break;

							instance.Activating = !instance.Activating;

							if (!instance.Activating)
								instance._lastActivator = default;
							else
							{
								instance._leverStopwatch.Restart();
								instance._lastActivator = new Footprinting.Footprint(ply);
							}

							instance._targetCooldown = instance._doorToggleCooldownTime;
						}
						break;
					case 2:
						if (instance.Activating && !instance.Engaged)
						{
							InteractGeneratorEvent ev = new(pl, instance.GetGenerator(), GeneratorStatus.Deactivate);
							ev.InvokeEvent();

							if (!ev.Allowed)
								break;

							instance.ServerSetFlag(Scp079Generator.GeneratorFlags.Activating, false);
							instance._targetCooldown = instance._unlockCooldownTime;
							instance._lastActivator = default;
						}
						break;
					default:
						instance._targetCooldown = 1f;
						break;
				}
				instance._cooldownStopwatch.Restart();
			}
			catch (Exception e)
			{
				Log.Error($"Patch Error - <Player> {{Interact}} [Generator]:{e}\n{e.StackTrace}");
			}
		}
	}
}