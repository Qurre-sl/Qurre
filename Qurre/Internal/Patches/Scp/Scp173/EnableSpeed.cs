namespace Qurre.Internal.Patches.Scp.Scp173;

using HarmonyLib;
using PlayerRoles.PlayableScps.Scp173;
using Qurre.API;
using Qurre.Events.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

[HarmonyPatch(typeof(Scp173BreakneckSpeedsAbility), nameof(Scp173BreakneckSpeedsAbility.IsActive), MethodType.Setter)]
static class EnableSpeed
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();
        instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

        LocalBuilder @event = generator.DeclareLocal(typeof(Scp173EnableSpeedEvent));

        List<CodeInstruction> list = new(instructions);

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
            methodBase.Name.Contains("ExecuteEvent")) + 3;

        if (index < 3)
        {
            Log.Error($"Creating Patch error: <SCPs> {{Scp173}} [EnableSpeed]: Index - {index} < 3");
            return list.AsEnumerable();
        }

        list.InsertRange(index, new CodeInstruction[]
        {
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index]), // instance [Scp173BreakneckSpeedsAbility]
            new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Scp173BreakneckSpeedsAbility), nameof(Scp173BreakneckSpeedsAbility.Owner))),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new Type[] { typeof(ReferenceHub) })),

            new CodeInstruction(OpCodes.Ldarg_1), // value [bool]

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(Scp173EnableSpeedEvent))[0]),
            new CodeInstruction(OpCodes.Stloc_S, @event.LocalIndex), // var @event = ...;

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // if (!@event.Allowed) return;
            new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Scp173EnableSpeedEvent), nameof(Scp173EnableSpeedEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel),
        });

        return list.AsEnumerable();
    }
}
/* 
 * ...
 * 
 * Scp173EnableSpeedEvent @event = new(instance.Owner.GetPlayer(), value));
 * @event.InvokeEvent();
 * 
 * if (!@event.Allowed) {
 *     return;
 * }
 * 
 * ...
 */