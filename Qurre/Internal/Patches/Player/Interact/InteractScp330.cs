using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Interactables.Interobjects;
using InventorySystem.Items.Usables.Scp330;
using Qurre.API;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.Player.Interact
{
    [HarmonyPatch(typeof(Scp330Interobject), nameof(Scp330Interobject.ServerInteract))]
    internal static class InteractScp330
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            LocalBuilder @event = generator.DeclareLocal(typeof(InteractScp330Event));

            List<CodeInstruction> list = new (instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase && methodBase.Name == nameof(Scp330Bag.ServerProcessPickup))
                        - 3;

            if (index < 0)
            {
                Log.Error($"Creating Patch error: <Player> {{Interact}} [Scp330]: Index - {index} < 0");
                return list.AsEnumerable();
            }

            list.InsertRange(
                index, new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(list[index]), // ply [ReferenceHub]
                    new (OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new[] { typeof(ReferenceHub) })),

                    new CodeInstruction(OpCodes.Ldarg_0),

                    new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(InteractScp330Event))[0]),
                    new CodeInstruction(OpCodes.Stloc_S, @event.LocalIndex),

                    new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                    new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(InteractScp330Event), nameof(InteractScp330Event.Allowed))),
                    new CodeInstruction(OpCodes.Brfalse, retLabel)
                });

            return list.AsEnumerable();
        }
    }
}