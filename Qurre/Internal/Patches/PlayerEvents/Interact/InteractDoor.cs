using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.PlayerEvents.Interact;

[HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal static class InteractDoor
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        LocalBuilder @event = generator.DeclareLocal(typeof(InteractDoorEvent));

        List<CodeInstruction> list = [..instructions];

        int index = list.FindIndex(ins => ins.opcode == OpCodes.Stloc_0) + 1;

        if (index < 1)
        {
            Log.Error($"Creating Patch error: <Player> {{Interact}} [Door]: Index - {index} < 1");
            return list.AsEnumerable();
        }

        int delIndex = list.FindLastIndex(ins =>
            ins.opcode == OpCodes.Call && ins.operand is MethodBase methodBase &&
            methodBase.Name.Contains("ExecuteEvent")) + 3;

        if (delIndex < 3)
        {
            Log.Error($"Creating Patch error: <Player> {{Interact}} [Door]: Del Index - {delIndex} < 3");
            return list.AsEnumerable();
        }

        list.RemoveRange(index, delIndex - index);

        list.InsertRange(index,
        [
            new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(list[index]), // ply [ReferenceHub]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Ldarg_0), // this [DoorVariant]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetDoor), [typeof(DoorVariant)])),

            new CodeInstruction(OpCodes.Ldloc_0), // allowed [bool]

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(InteractDoorEvent))[0]),
            new CodeInstruction(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // allowed = @event.Allowed;
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(InteractDoorEvent), nameof(InteractDoorEvent.Allowed))),
            new CodeInstruction(OpCodes.Stloc_0)
        ]);


        delIndex = list.FindLastIndex(ins => ins.opcode == OpCodes.Call &&
                                             ins.operand is MethodBase methodBase &&
                                             methodBase.Name.Contains("ExecuteEvent")) - 4;

        var pasteLabels = list[delIndex].ExtractLabels();

        list.RemoveRange(delIndex, 5);
        list.InsertRange(delIndex,
        [
            new CodeInstruction(OpCodes.Ldarg_1).WithLabels(pasteLabels), // ply [ReferenceHub]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), [typeof(ReferenceHub)])),

            new CodeInstruction(OpCodes.Ldarg_0), // this [DoorVariant]
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(Extensions), nameof(Extensions.GetDoor), [typeof(DoorVariant)])),

            new CodeInstruction(OpCodes.Ldc_I4_0), // false [bool]

            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(InteractDoorEvent))[0]),
            new CodeInstruction(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
            new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

            new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // allowed = @event.Allowed;
            new CodeInstruction(OpCodes.Callvirt,
                AccessTools.PropertyGetter(typeof(InteractDoorEvent), nameof(InteractDoorEvent.Allowed)))
        ]);

        return list.AsEnumerable();
    }
}