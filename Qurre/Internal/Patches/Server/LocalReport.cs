using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Server
{
    using Qurre.API;
    using Qurre.Events.Structs;

    [HarmonyDebug]
    [HarmonyPatch(typeof(global::CheaterReport), nameof(global::CheaterReport.UserCode_CmdReport))]
    static class LocalReport
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Call(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            Label retLabel = generator.DefineLabel();
            instructions.ElementAt(instructions.Count() - 1).labels.Add(retLabel);

            LocalBuilder @event = generator.DeclareLocal(typeof(LocalReportEvent));

            List<CodeInstruction> list = new(instructions);

            int index = -1;
            int end = -1;
            for (int i = 0; i < list.Count; i++)
            {
                var ins = list[i];

                if (ins.opcode == OpCodes.Ldarg_S && $"{ins.operand}" == "4" &&
                    list[i + 1].opcode == OpCodes.Brtrue)
                    index = i + 2;

                if (index > 0 && end < 0 && ins.opcode == OpCodes.Call && ins.operand is not null && ins.operand is MethodBase methodBase &&
                    methodBase.Name.Contains("ExecuteEvent"))
                    end = i;
            }

            if (index < 0)
            {
                Log.Error($"Creating Patch error: <Server> [LocalReport]: Index - {index} < 0");
                return list.AsEnumerable();
            }
            if (end < 0)
            {
                Log.Error($"Creating Patch error: <Server> [LocalReport]: End Index - {end} < 0");
                return list.AsEnumerable();
            }

            list[end + 3].ExtractLabels();
            list.RemoveRange(index, end - index + 3);
            list.InsertRange(index, new CodeInstruction[]
            {
                // issuer
                new CodeInstruction(OpCodes.Ldarg_0), // this
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(global::CheaterReport), nameof(global::CheaterReport._hub))), // ReferenceHub
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new[] { typeof(ReferenceHub) })),

                // target
                new CodeInstruction(OpCodes.Ldloc_2), // referenceHub
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Extensions), nameof(Extensions.GetPlayer), new[] { typeof(ReferenceHub) })),

                new CodeInstruction(OpCodes.Ldarg_2), // reason [string]

                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(LocalReportEvent))[0]),
                new CodeInstruction(OpCodes.Stloc, @event.LocalIndex), // var @event = ...;

                new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex), // @event.InvokeEvent();
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent))),

                // if(!@event.Allowed) return;
                new CodeInstruction(OpCodes.Ldloc_S, @event.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(LocalReportEvent), nameof(LocalReportEvent.Allowed))),
                new CodeInstruction(OpCodes.Brfalse, retLabel),
            });


            return list.AsEnumerable();
        }
        /*
         * ...
         * if (notifyGm) { ...; return; }
         * 
         * var @event = new LocalReportEvent(this._hub.GetPlayer(), referenceHub.GetPlayer(), reason);
         * @event.InvokeEvent();
         * if(!@event.Allowed) return;
         * 
         * GameCore.Console.AddLog(...);
         * ...
        */
    }
}