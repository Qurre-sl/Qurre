using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Qurre.API;
using Qurre.Events.Structs;

namespace Qurre.Internal.Patches.Player.Role
{
    [HarmonyPatch(typeof(global::Escape), nameof(global::Escape.ServerHandlePlayer))]
    internal static class Escape
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            LocalBuilder @event = generator.DeclareLocal(typeof(EscapeEvent));

            List<CodeInstruction> list = new (instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase && methodBase.Name.Contains("ExecuteEvent")) + 3;

            if (index < 3)
            {
                Log.Error($"Creating Patch error: <Player> {{Role}} [Escape]: Index - {index} < 3");
                return list.AsEnumerable();
            }

            list.InsertRange(
                index, new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index]), // hub [ReferenceHub]
                    new (OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new[] { typeof(ReferenceHub) })),

                    new CodeInstruction(OpCodes.Ldloc_0),

                    new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(EscapeEvent))[0]),
                    new CodeInstruction(OpCodes.Stloc_S, @event.LocalIndex), // var @event = ...;

                    new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                    new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // if(!@event.Allowed) return;
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(EscapeEvent), nameof(EscapeEvent.Allowed))),
                    new CodeInstruction(OpCodes.Brfalse, retLabel),

                    new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // roleTypeId = @event.Role;
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(EscapeEvent), nameof(EscapeEvent.Role))),
                    new CodeInstruction(OpCodes.Stloc_0)
                });

            return list.AsEnumerable();
        }
    }
}