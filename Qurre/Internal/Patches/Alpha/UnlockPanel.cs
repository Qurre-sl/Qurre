using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Alpha
{
    using Qurre.API;
    using Qurre.Events.Structs;

    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdSwitchAWButton))]
    static class UnlockPanel
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            LocalBuilder @event = generator.DeclareLocal(typeof(UnlockPanelEvent));

            List<CodeInstruction> list = new(instructions);

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                methodBase.Name == nameof(PlayerInteract.OnInteract));

            if (index < 1)
            {
                Log.Error($"Creating Patch error: <Alpha> [UnlockPanel]: Index - {index} < 1");
                return list.AsEnumerable();
            }

            list.InsertRange(index - 1, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(list[index - 1]), // this
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PlayerInteract), nameof(PlayerInteract._hub))), // ReferenceHub
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UnlockPanelEvent))[0]),
                new CodeInstruction(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

                new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                // if(!@event.Allowed) return;
                new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UnlockPanelEvent), nameof(UnlockPanelEvent.Allowed))),
                new CodeInstruction(OpCodes.Brfalse, retLabel),
            });

            return list.AsEnumerable();
        }
        /*
         * ...
         * < if(...) return; >
         * 
         * var @event = new UnlockPanelEvent(this._hub.GetPlayer());
         * @event.InvokeEvent();
         * if(!@event.Allowed) return;
         * 
         * this.OnInteract();
         * ...
        */
    }
}