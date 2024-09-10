using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp173;
using Qurre.API;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.ScpEvents.Scp173;

[HarmonyPatch(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.IsObservedBy))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class AddObserver
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label retTrueLabel = generator.DefineLabel();

        LocalBuilder @event = generator.DeclareLocal(typeof(Scp173AddObserverEvent));

        List<CodeInstruction> list = [..instructions];

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Call &&
                                          ins.operand is MethodBase methodBase &&
                                          methodBase.Name.Contains("ExecuteEvent")) - 4;

        if (index < 0)
        {
            Log.Error($"Creating Patch error: <SCPs> {{Scp173}} [AddObserver]: Index - {index} < 0");
            return list.AsEnumerable();
        }

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(list[index]), // target [ReferenceHub]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Ldarg_0), // instance [Scp173ObserversTracker]
            new CodeInstruction(OpCodes.Call,
                AccessTools.PropertyGetter(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.Owner))),
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(Scp173AddObserverEvent))[0]),
            new CodeInstruction(OpCodes.Stloc_S, @event.LocalIndex), // var @event = ...;

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // return @event.Allowed;
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(Scp173AddObserverEvent), nameof(Scp173AddObserverEvent.Allowed))),
            new CodeInstruction(OpCodes.Brtrue, retTrueLabel),

            new CodeInstruction(OpCodes.Ldc_I4_0),
            new CodeInstruction(OpCodes.Ret),

            new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(retTrueLabel),
            new CodeInstruction(OpCodes.Ret)
        ]);

        return list.AsEnumerable();
    }
}
/*
 * Scp173AddObserverEvent @event = new(target.GetPlayer(), instance.Owner.GetPlayer());
 * @event.InvokeEvent();
 *
 * if (@event.Allowed) {
 *     return true;
 * }
 *
 * return false;
 */