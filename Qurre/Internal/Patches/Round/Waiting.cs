using HarmonyLib;
using MapGeneration;
using System.Collections.Generic;
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

            int index = list.FindIndex(ins => ins.opcode == OpCodes.Stsfld && ins.operand is not null && ins.operand is FieldInfo methodBase &&
                methodBase.Name == nameof(SeedSynchronizer.MapGenerated)) + 1;

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