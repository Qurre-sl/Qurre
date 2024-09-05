using HarmonyLib;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Player.Pickups
{
    using Qurre.API;
    using Qurre.API.Controllers;
    using Qurre.Events.Structs;
    using Qurre.Internal.EventsManager;

    [HarmonyPatch(typeof(AmmoSearchCompletor), nameof(AmmoSearchCompletor.Complete))]
    static class PickupAmmo
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            List<CodeInstruction> list = new(instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name.Contains("ExecuteEvent")) + 3;

            if (index < 3)
            {
                Log.Error($"Creating Patch error: <Player> {{Pickups}} [PickupAmmo]: Index - {index} < 3");
                return list.AsEnumerable();
            }

            list.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index]),

                new CodeInstruction(OpCodes.Ldloc_0),

                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PickupAmmo), nameof(PickupAmmo.Invoke))),
                new CodeInstruction(OpCodes.Brfalse, retLabel),
            });

            return list.AsEnumerable();
        }

        static bool Invoke(AmmoSearchCompletor instance, AmmoPickup ammo)
        {
            try
            {
                PickupAmmoEvent ev = new(instance.Hub.GetPlayer(), Pickup.SafeGet(instance.TargetPickup), ammo);
                ev.InvokeEvent();

                if (!ev.Allowed)
                {
                    var info = instance.TargetPickup.Info;
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