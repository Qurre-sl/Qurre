using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.Events.Structs;
using Qurre.Internal.EventsManager;

namespace Qurre.Internal.Patches.Player.Pickups
{
    [HarmonyPatch(typeof(AmmoSearchCompletor), nameof(AmmoSearchCompletor.Complete))]
    internal static class PickupAmmo
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            List<CodeInstruction> list = new (instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase && methodBase.Name.Contains("ExecuteEvent")) + 3;

            if (index < 3)
            {
                Log.Error($"Creating Patch error: <Player> {{Pickups}} [PickupAmmo]: Index - {index} < 3");
                return list.AsEnumerable();
            }

            list.InsertRange(
                index, new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index]),

                    new (OpCodes.Ldloc_0),

                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PickupAmmo), nameof(Invoke))),
                    new CodeInstruction(OpCodes.Brfalse, retLabel)
                });

            return list.AsEnumerable();
        }

        private static bool Invoke(AmmoSearchCompletor instance, AmmoPickup ammo)
        {
            try
            {
                PickupAmmoEvent ev = new (instance.Hub.GetPlayer(), Pickup.SafeGet(instance.TargetPickup), ammo);
                ev.InvokeEvent();

                if (!ev.Allowed)
                {
                    PickupSyncInfo info = instance.TargetPickup.Info;
                    info.InUse = false;
                    info.Locked = false;
                    instance.TargetPickup.NetworkInfo = info;
                }

                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"Patch Error - <Player> {{Pickups}} [PickupAmmo]: {e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}