using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.AlphaEvents;

[HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.StartDetonation))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Start
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        LocalBuilder @event = generator.DeclareLocal(typeof(AlphaStartEvent));

        List<CodeInstruction> list = [..instructions];
        list.Last().labels.Add(retLabel);

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Stfld && ins.operand is FieldInfo
        {
            Name: nameof(AlphaWarheadController._isAutomatic)
        });

        if (index < 2)
        {
            Log.Error($"Creating Patch error: <Alpha> [Start]: Index - {index} < 2");
            return list.AsEnumerable();
        }

        list.InsertRange(index - 2,
        [
            new CodeInstruction(OpCodes.Ldarg_3).MoveLabelsFrom(list[index - 2]), // trigger [ReferenceHub]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),
            new CodeInstruction(OpCodes.Ldarg_1), // isAutomatic [bool]
            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(AlphaStartEvent))[0]),
            new CodeInstruction(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            // isAutomatic = @event.Automatic;
            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(AlphaStartEvent), nameof(AlphaStartEvent.Automatic))),
            new CodeInstruction(OpCodes.Starg_S, 1),

            // if(!@event.Allowed) return;
            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(AlphaStartEvent), nameof(AlphaStartEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);

        return list.AsEnumerable();
    }
}
/*
 * ...
 * < if(...) return; >
 *
 * var @event = new AlphaStopEvent(disabler.GetPlayer(), isAutomatic);
 * @event.InvokeEvent();
 * isAutomatic = @event.Automatic;
 * if(!@event.Allowed) return;
 *
 * this._isAutomatic = isAutomatic;
 * ...
 */