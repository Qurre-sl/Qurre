using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Player.Interact
{
    using Qurre.API;
    using Qurre.Events.Structs;

    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract))]
    static class InteractDoor
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            LocalBuilder @event = generator.DeclareLocal(typeof(InteractDoorEvent));

            List<CodeInstruction> list = new(instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Stloc_0) + 1;

            if (index < 1)
            {
                Log.Error($"Creating Patch error: <Player> {{Interact}} [Door]: Index - {index} < 1");
                return list.AsEnumerable();
            }

            int delIndex = list.FindLastIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
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
                new(OpCodes.Call, AccessTools.Method( typeof(Extensions), nameof(Extensions.GetPlayer), [ typeof(ReferenceHub) ] )),

                new(OpCodes.Ldarg_0), // this [DoorVariant]
                new(OpCodes.Call, AccessTools.Method( typeof(Extensions), nameof(Extensions.GetDoor), [ typeof(DoorVariant) ] )),

                new(OpCodes.Ldloc_0), // allowed [bool]

                new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(InteractDoorEvent))[0]),
                new(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

                new(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
                new(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                new(OpCodes.Ldloc_S, @event.LocalIndex), // allowed = @event.Allowed;
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(InteractDoorEvent), nameof(InteractDoorEvent.Allowed))),
                new(OpCodes.Stloc_0),
            ]);


            delIndex = list.FindLastIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name.Contains("ExecuteEvent")) - 4;

            List<Label> pasteLabels = list[delIndex].ExtractLabels();

            list.RemoveRange(delIndex, 5);
            list.InsertRange(delIndex,
            [
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(pasteLabels), // ply [ReferenceHub]
                new(OpCodes.Call, AccessTools.Method( typeof(Extensions), nameof(Extensions.GetPlayer), [ typeof(ReferenceHub) ] )),

                new(OpCodes.Ldarg_0), // this [DoorVariant]
                new(OpCodes.Call, AccessTools.Method( typeof(Extensions), nameof(Extensions.GetDoor), [ typeof(DoorVariant) ] )),

                new(OpCodes.Ldc_I4_0), // false [bool]

                new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(InteractDoorEvent))[0]),
                new(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

                new(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
                new(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                new(OpCodes.Ldloc_S, @event.LocalIndex), // allowed = @event.Allowed;
                new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(InteractDoorEvent), nameof(InteractDoorEvent.Allowed))),
            ]);

            return list.AsEnumerable();
        }
    }
}