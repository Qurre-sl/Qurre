namespace Qurre.Internal.Patches.Scp.Scp096;

using HarmonyLib;
using PlayerRoles.PlayableScps.Scp096;
using Qurre.API;
using Qurre.Events.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

[HarmonyPatch(typeof(Scp096StateController), nameof(Scp096StateController.SetRageState))]
static class StateRageUpdate
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();
        instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

        LocalBuilder @event = generator.DeclareLocal(typeof(Scp096SetStateEvent));

        List<CodeInstruction> list = new(instructions);

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
            methodBase.Name.Contains("ExecuteEvent")) - 3;

        if (index < 0)
        {
            Log.Error($"Creating Patch error: <SCPs> {{Scp096}} [StateRageUpdate]: Index - {index} < 0");
            return list.AsEnumerable();
        }

        list.InsertRange(index, new CodeInstruction[]
        {
            new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(list[index]), // hub [ReferenceHub]
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new Type[] { typeof(ReferenceHub) })),

            new CodeInstruction(OpCodes.Ldarg_1), // state [Scp096RageState]

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(Scp096SetStateEvent))[1]),
            new CodeInstruction(OpCodes.Stloc_S, @event.LocalIndex), // var @event = ...;

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // if (!@event.Allowed) return;
            new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Scp096SetStateEvent), nameof(Scp096SetStateEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel),
        });

        return list.AsEnumerable();
    }
}