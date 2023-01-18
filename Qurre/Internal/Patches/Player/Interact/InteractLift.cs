using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Interactables.Interobjects;
using Qurre.API;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.Player.Interact
{
    [HarmonyPatch(typeof(ElevatorManager), nameof(ElevatorManager.ServerReceiveMessage))]
    internal static class InteractLift
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            LocalBuilder @event = generator.DeclareLocal(typeof(InteractLiftEvent));

            List<CodeInstruction> list = new (instructions);

            int index = list.FindIndex(
                            ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase && methodBase.Name == nameof(ElevatorManager.TrySetDestination))
                        - 3;

            if (index < 0)
            {
                Log.Error($"Creating Patch error: <Player> {{Interact}} [Lift]: Index - {index} < 0");
                return list.AsEnumerable();
            }

            list.InsertRange(
                index, new[]
                {
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(list[index]), // hub [ReferenceHub]
                    new (OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new[] { typeof(ReferenceHub) })),

                    new CodeInstruction(OpCodes.Ldloc_3), // [ElevatorChamber]
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetLift), new[] { typeof(ElevatorChamber) })),

                    new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(InteractLiftEvent))[0]),
                    new CodeInstruction(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

                    new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                    // if(!@event.Allowed) return;
                    new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(InteractLiftEvent), nameof(InteractLiftEvent.Allowed))),
                    new CodeInstruction(OpCodes.Brfalse, retLabel)
                });

            return list.AsEnumerable();
        }
    }
}