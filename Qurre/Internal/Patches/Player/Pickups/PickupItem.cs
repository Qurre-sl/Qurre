using HarmonyLib;
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

    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    static class PickupItem
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            List<CodeInstruction> list = new(instructions);

            int delIndex = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name.Contains("ExecuteEvent")) + 3;

            if (delIndex < 3)
            {
                Log.Error($"Creating Patch error: <Player> {{Pickups}} [PickupItem]: Del Index - {delIndex} < 3");
                return list.AsEnumerable();
            }

            list.RemoveRange(0, delIndex);

            list[0].ExtractLabels();

            list.InsertRange(0, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PickupItem), nameof(PickupItem.Invoke))),
                new CodeInstruction(OpCodes.Brfalse, retLabel),
            });

            return list.AsEnumerable();
        }

        static bool Invoke(ItemSearchCompletor instance)
        {
            try
            {
                PickupItemEvent ev = new(instance.Hub.GetPlayer(), Pickup.SafeGet(instance.TargetPickup));
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
                Log.Error($"Patch Error - <Player> {{Pickups}} [PickupItem]: {e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}