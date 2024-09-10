using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.AlphaEvents;

[HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.CancelDetonation), typeof(ReferenceHub))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class Stop
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retLabel = generator.DefineLabel();

        LocalBuilder @event = generator.DeclareLocal(typeof(AlphaStopEvent));

        List<CodeInstruction> list = [..instructions];
        list.Last().labels.Add(retLabel);

        int index = list.FindIndex(ins =>
            ins.opcode == OpCodes.Ldstr && (ins.operand as string ?? string.Empty).Contains("cancelled"));

        if (index < 1)
        {
            Log.Error($"Creating Patch error: <Alpha> [Stop]: Index - {index} < 1");
            return list.AsEnumerable();
        }

        list.InsertRange(index - 1,
        [
            new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(list[index - 1]), // disabler [ReferenceHub]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),
            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(AlphaStopEvent))[0]),
            new CodeInstruction(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            // if(!@event.Allowed) return;
            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(AlphaStopEvent), nameof(AlphaStopEvent.Allowed))),
            new CodeInstruction(OpCodes.Brfalse, retLabel)
        ]);

        return list.AsEnumerable();
    }
}
/*
 * ...
 * < if(...) return; >
 *
 * var @event = new AlphaStopEvent(disabler.GetPlayer());
 * @event.InvokeEvent();
 * if(!@event.Allowed) return;
 *
 * ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Detonation cancelled.", ServerLogs.ServerLogType.GameEvent);
 * ...
 */