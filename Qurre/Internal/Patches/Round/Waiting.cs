using HarmonyLib;
using MapGeneration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Qurre.Internal.Patches.Round
{
    using Qurre.Events.Structs;

    [HarmonyPatch(typeof(SeedSynchronizer), nameof(SeedSynchronizer.Update))]
    static class Waiting
    {
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new(instructions);

            int index = list.FindLastIndex(ins => ins.opcode == OpCodes.Callvirt && ins.operand is not null && ins.operand is MethodInfo methodBase &&
                methodBase.Name == nameof(Stopwatch.Restart)) + 1;

            if (0 >= index)
            {
                index = instructions.Count() - 1;
            }

            list.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(WaitingEvent))[0]),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EventsManager.Loader), nameof(EventsManager.Loader.InvokeEvent)))
            });

            return list.AsEnumerable();
        }
        /* ...
         * MapGenerated = true; // original instruction;
         * Loader.InvokeEvent(new WaitingEvent());
         * ...
         */
    }
}