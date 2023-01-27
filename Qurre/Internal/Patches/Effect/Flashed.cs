using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Qurre.API;
using Qurre.Events.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Effect
{
    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.ProcessPlayer))]
    static class Flashed
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            LocalBuilder @event = generator.DeclareLocal(typeof(PlayerFlashedEvent));

            List<CodeInstruction> list = new(instructions);

            int index = -1;

            bool found = false;
            for (int i = 0; i < list.Count && index < 0; i++)
            {
                var ins = list[i];
                if (!found && ins.opcode == OpCodes.Callvirt && ins.operand is not null && ins.operand is MethodBase methodBase &&
                    methodBase.Name == nameof(PlayerEffectsController.EnableEffect))
                    found = true;
                else if (found && ins.opcode == OpCodes.Ldloc_2)
                    index = i;
            }

            if (index < 0)
            {
                Log.Error($"Creating Patch error: <Effect> {{Player}} [Flashed]: Index - {index} < 0");
                return list.AsEnumerable();
            }

            list.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(list[index]), // hub [ReferenceHub]
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new Type[] { typeof(ReferenceHub) })),

                new CodeInstruction(OpCodes.Ldarg_0), // this [FlashbangGrenade]
                
                new CodeInstruction(OpCodes.Ldloc_2), // num2 [float]

                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(PlayerFlashedEvent))[0]),
                new CodeInstruction(OpCodes.Stloc_S, @event.LocalIndex), // var @event = ...;

                new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // if(!@event.Allowed) return;
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerFlashedEvent), nameof(PlayerFlashedEvent.Allowed))),
                new CodeInstruction(OpCodes.Brfalse, retLabel),
            });

            return list.AsEnumerable();
        }
    }
}